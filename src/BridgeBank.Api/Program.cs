using Simansoft.BridgeBank.Api.Dtos;
using Simansoft.BridgeBank.Api.Mappers;
using Simansoft.BridgeBank.Core;
using Simansoft.BridgeBank.Core.Classificacao;
using Simansoft.BridgeBank.Core.Models;
using Simansoft.BridgeBank.Core.Strategies;
using Simansoft.BridgeBank.Generators.Bancos;
using Simansoft.BridgeBank.Generators.Interfaces;
using Simansoft.BridgeBank.Parsers.Bancos;
using Simansoft.BridgeBank.Parsers.Interfaces;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Register parsers
builder.Services.AddSingleton<ILeitorExtrato, LeitorExtratoBCI>();
builder.Services.AddSingleton<ILeitorExtrato, LeitorExtratoMillenniumBIM>();
builder.Services.AddSingleton<ILeitorExtrato, LeitorExtratoStandardBank>();
builder.Services.AddSingleton<ILeitorExtrato, LeitorExtratoNedBank>();
builder.Services.AddSingleton<ILeitorExtrato, LeitorExtratoAccessBank>();
builder.Services.AddSingleton<ILeitorExtrato, LeitorExtratoMPesa>();

// Register generators
builder.Services.AddSingleton<IGeradorFicheiroPagamento, GeradorPagamentoBCI>();
builder.Services.AddSingleton<IGeradorFicheiroPagamento, GeradorPagamentoMillenniumBIM>();
builder.Services.AddSingleton<IGeradorFicheiroPagamento, GeradorPagamentoStandardBank>();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();
app.UseCors();

// ─── Health ───────────────────────────────────────────────────────────
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .WithTags("Health");

// ─── Bancos Suportados ────────────────────────────────────────────────
app.MapGet("/api/bancos/parsers", (IEnumerable<ILeitorExtrato> leitores) =>
{
    var bancos = leitores.Select(l => l.GetType().Name.Replace("LeitorExtrato", "")).ToList();
    return Results.Ok(bancos);
})
.WithName("ListarParsers")
.WithTags("Bancos");

app.MapGet("/api/bancos/geradores", (IEnumerable<IGeradorFicheiroPagamento> geradores) =>
{
    var bancos = geradores.Select(g => new { Banco = g.GetType().Name.Replace("GeradorPagamento", ""), g.FormatoFicheiro }).ToList();
    return Results.Ok(bancos);
})
.WithName("ListarGeradores")
.WithTags("Bancos");

// ─── Extratos (Parsing) ──────────────────────────────────────────────
app.MapPost("/api/extratos/parse", async (HttpRequest request, IEnumerable<ILeitorExtrato> leitores) =>
{
    var form = await request.ReadFormAsync();
    var file = form.Files.FirstOrDefault();
    if (file is null || file.Length == 0)
        return Results.BadRequest(new { error = "Nenhum ficheiro enviado." });

    var tempPath = Path.Combine(Path.GetTempPath(), $"bridgebank_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");
    try
    {
        await using (var stream = File.Create(tempPath))
        {
            await file.CopyToAsync(stream);
        }

        var leitor = leitores.FirstOrDefault(l => l.SuportaArquivo(tempPath));
        if (leitor is null)
            return Results.BadRequest(new { error = "Formato de ficheiro não suportado.", bancosSuportados = leitores.Select(l => l.GetType().Name.Replace("LeitorExtrato", "")).ToList() });

        var extrato = leitor.LerExtrato(tempPath);
        return Results.Ok(extrato.ToDto());
    }
    finally
    {
        if (File.Exists(tempPath))
            File.Delete(tempPath);
    }
})
.DisableAntiforgery()
.WithName("ParseExtrato")
.WithTags("Extratos")
.Accepts<IFormFile>("multipart/form-data")
.Produces<ExtratoBancarioDto>(200);

// ─── Classificação ───────────────────────────────────────────────────
app.MapPost("/api/classificacao", (ClassificarTransacoesRequest request) =>
{
    using var classificador = ClassificadorTransacao.CriarComRegrasPadrao();

    var resultados = request.Transacoes.Select(dto =>
    {
        var transacao = dto.ToModel();
        var resultado = classificador.Classificar(transacao);
        return new
        {
            Transacao = transacao.ToDto(),
            Classificacao = resultado.ToDto()
        };
    }).ToList();

    return Results.Ok(resultados);
})
.WithName("ClassificarTransacoes")
.WithTags("Classificação");

app.MapPost("/api/classificacao/extrato", async (HttpRequest request, IEnumerable<ILeitorExtrato> leitores) =>
{
    var form = await request.ReadFormAsync();
    var file = form.Files.FirstOrDefault();
    if (file is null || file.Length == 0)
        return Results.BadRequest(new { error = "Nenhum ficheiro enviado." });

    var tempPath = Path.Combine(Path.GetTempPath(), $"bridgebank_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");
    try
    {
        await using (var stream = File.Create(tempPath))
        {
            await file.CopyToAsync(stream);
        }

        var leitor = leitores.FirstOrDefault(l => l.SuportaArquivo(tempPath));
        if (leitor is null)
            return Results.BadRequest(new { error = "Formato de ficheiro não suportado." });

        var extrato = leitor.LerExtrato(tempPath);
        using var classificador = ClassificadorTransacao.CriarComRegrasPadrao();
        var resultados = classificador.ClassificarExtrato(extrato);

        return Results.Ok(new
        {
            Extrato = extrato.ToDto(),
            Classificacoes = resultados.Select(r => r.ToDto()).ToList()
        });
    }
    finally
    {
        if (File.Exists(tempPath))
            File.Delete(tempPath);
    }
})
.DisableAntiforgery()
.WithName("ClassificarExtrato")
.WithTags("Classificação")
.Accepts<IFormFile>("multipart/form-data");

// ─── Reconciliação ───────────────────────────────────────────────────
app.MapPost("/api/reconciliacao", (ReconciliarRequest request) =>
{
    var motor = new MotorReconciliacao();
    motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorReferencia());
    motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorValorEData());
    motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorDescricao());

    var transacoes = request.Transacoes.Select(t => t.ToModel()).ToList();
    var lancamentos = request.LancamentosERP.Select(l => l.ToModel()).ToList();

    var resultados = motor.Reconciliar(transacoes, lancamentos);

    return Results.Ok(resultados.Select(r => r.ToDto()).ToList());
})
.WithName("Reconciliar")
.WithTags("Reconciliação");

// ─── Geração de Pagamentos ──────────────────────────────────────────
app.MapPost("/api/pagamentos/gerar", (GerarPagamentoRequest request, IEnumerable<IGeradorFicheiroPagamento> geradores) =>
{
    var gerador = geradores.FirstOrDefault(g =>
        g.GetType().Name.Contains(request.Banco, StringComparison.OrdinalIgnoreCase));

    if (gerador is null)
        return Results.BadRequest(new
        {
            error = $"Gerador para o banco '{request.Banco}' não encontrado.",
            bancosSuportados = geradores.Select(g => g.GetType().Name.Replace("GeradorPagamento", "")).ToList()
        });

    var pagamentos = request.Pagamentos.Select(p => p.ToModel()).ToList();
    var tempPath = Path.Combine(Path.GetTempPath(), $"pagamento_{Guid.NewGuid()}.{gerador.FormatoFicheiro}");

    try
    {
        gerador.GerarFicheiro(pagamentos, tempPath);
        var bytes = File.ReadAllBytes(tempPath);
        var contentType = gerador.FormatoFicheiro.ToLowerInvariant() switch
        {
            "xml" => "application/xml",
            "csv" => "text/csv",
            _ => "text/plain"
        };
        return Results.File(bytes, contentType, $"pagamento.{gerador.FormatoFicheiro}");
    }
    finally
    {
        if (File.Exists(tempPath))
            File.Delete(tempPath);
    }
})
.WithName("GerarPagamento")
.WithTags("Pagamentos");

// ─── Enumerações (referência) ────────────────────────────────────────
app.MapGet("/api/enums/categorias", () =>
    Results.Ok(Enum.GetNames<CategoriaTransacao>()))
.WithName("ListarCategorias")
.WithTags("Referência");

app.MapGet("/api/enums/tipos-transacao", () =>
    Results.Ok(Enum.GetNames<TipoTransacao>()))
.WithName("ListarTiposTransacao")
.WithTags("Referência");

app.MapGet("/api/enums/tipos-correspondencia", () =>
    Results.Ok(Enum.GetNames<TipoCorrespondencia>()))
.WithName("ListarTiposCorrespondencia")
.WithTags("Referência");

app.Run();
