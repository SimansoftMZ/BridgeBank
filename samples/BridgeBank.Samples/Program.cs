using BridgeBank.Samples;
using Simansoft.BridgeBank.Core.Models;
using Simansoft.BridgeBank.Parsers.Bancos;
using Simansoft.BridgeBank.Parsers.Interfaces;

// ── Passo 1: Carregar extracto bancário ──────────────────────────────
Console.WriteLine("BridgeBank - Exemplos de Utilização");
Console.WriteLine(new string('=', 50));
Console.WriteLine();
Console.WriteLine("Antes de começar, carregue um extracto bancário.");
Console.WriteLine();

// Escolher banco
Console.WriteLine("Bancos disponíveis:");
Console.WriteLine("  1. BCI (Banco Comercial e de Investimentos)");
Console.WriteLine("  2. Millennium BIM");
Console.WriteLine("  3. Standard Bank");
Console.WriteLine("  4. NedBank");
Console.WriteLine("  5. Access Bank");
Console.WriteLine("  6. M-Pesa");
Console.WriteLine();
Console.Write("Escolha o banco: ");

string opcaoBanco = Console.ReadLine()?.Trim() ?? "";

Dictionary<string, Func<ILeitorExtrato>> leitores = new()
{
    ["1"] = () => new LeitorExtratoBCI(),
    ["2"] = () => new LeitorExtratoMillenniumBIM(),
    ["3"] = () => new LeitorExtratoStandardBank(),
    ["4"] = () => new LeitorExtratoNedBank(),
    ["5"] = () => new LeitorExtratoAccessBank(),
    ["6"] = () => new LeitorExtratoMPesa(),
};

if (!leitores.TryGetValue(opcaoBanco, out Func<ILeitorExtrato>? criarLeitor))
{
    Console.WriteLine("Opção inválida. A encerrar.");
    return;
}

Console.Write("Caminho do ficheiro de extracto: ");
string caminho = Console.ReadLine()?.Trim()?.Trim('"') ?? "";

if (!File.Exists(caminho))
{
    Console.WriteLine($"Ficheiro não encontrado: {caminho}");
    return;
}

ILeitorExtrato leitor = criarLeitor();

if (!leitor.SuportaArquivo(caminho))
{
    Console.WriteLine("O leitor seleccionado não suporta este tipo de ficheiro.");
    return;
}

ExtratoBancario extrato;
try
{
    extrato = leitor.LerExtrato(caminho);
}
catch (Exception ex)
{
    Console.WriteLine($"Erro ao ler o extracto: {ex.Message}");
    return;
}

Console.WriteLine();
Console.WriteLine($"Extracto carregado: {extrato.Banco} — {extrato.Transacoes.Count} transacções");
Console.WriteLine();
Console.Write("Prima qualquer tecla para continuar...");
Console.ReadKey(true);

// ── Passo 2: Menu principal ──────────────────────────────────────────
while (true)
{
    Console.Clear();
    Console.WriteLine("BridgeBank - Exemplos de Utilização");
    Console.WriteLine(new string('=', 50));
    Console.WriteLine($"  Extracto: {extrato.Banco} | {extrato.Transacoes.Count} transacções");
    Console.WriteLine(new string('=', 50));
    Console.WriteLine();
    Console.WriteLine("  1. Leitura de extractos bancários (Parsers)");
    Console.WriteLine("  2. Reconciliação bancária (Core)");
    Console.WriteLine("  3. Geração de ficheiros de pagamento (Generators)");
    Console.WriteLine("  4. Classificação automática de transações (Core)");
    Console.WriteLine("  5. Classificação ML.NET vs Regras (comparação)");
    Console.WriteLine();
    Console.WriteLine("  0. Sair");
    Console.WriteLine();
    Console.Write("Escolha uma opção: ");

    string? opcao = Console.ReadLine()?.Trim();

    Console.WriteLine();

    switch (opcao)
    {
        case "1":
            ExemploParsers.Executar();
            break;
        case "2":
            ExemploReconciliacao.Executar(extrato);
            break;
        case "3":
            ExemploGeradores.Executar();
            break;
        case "4":
            ExemploClassificacao.Executar(extrato);
            break;
        case "5":
            ExemploClassificacaoML.Executar(extrato);
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Opção inválida.");
            break;
    }

    Console.WriteLine();
    Console.Write("Prima qualquer tecla para voltar ao menu...");
    Console.ReadKey(true);
}
