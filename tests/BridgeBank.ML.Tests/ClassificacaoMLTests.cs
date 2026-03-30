using Simansoft.BridgeBank.Core.Models;
using Simansoft.BridgeBank.ML.Classificacao;
using Simansoft.BridgeBank.ML.Dados;

namespace BridgeBank.ML.Tests;

[TestClass]
public class ClassificacaoMLTests
{
    private static string _caminhoModelo = null!;

    [ClassInitialize]
    public static void ClassInit(TestContext _)
    {
        _caminhoModelo = Path.Combine(Path.GetTempPath(), "bridgebank-test", "classificacao-test.zip");

        var treinador = new TreinadorClassificacao();
        var metricas = treinador.TreinarComDadosSinteticos(_caminhoModelo, exemplosPorCategoria: 100);

        Assert.IsTrue(metricas.MicroAccuracy > 0.5, $"MicroAccuracy muito baixa: {metricas.MicroAccuracy:P2}");
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        if (File.Exists(_caminhoModelo))
            File.Delete(_caminhoModelo);
    }

    // ── Treino e modelo ─────────────────────────────────────────────

    [TestMethod]
    public void TreinarComDadosSinteticos_DeveCriarFicheiroModelo()
    {
        Assert.IsTrue(File.Exists(_caminhoModelo));
    }

    [TestMethod]
    public void GerarDadosClassificacao_DeveGerarQuantidadeCorrecta()
    {
        var dados = GeradorDadosTreino.GerarDadosClassificacao(50);
        // 9 categorias * 50 exemplos = 450
        Assert.AreEqual(450, dados.Count);
    }

    [TestMethod]
    public void GerarDadosClassificacao_DeveConterTodasCategorias()
    {
        var dados = GeradorDadosTreino.GerarDadosClassificacao(10);
        var categorias = dados.Select(d => d.Categoria).Distinct().ToList();

        Assert.IsTrue(categorias.Contains(nameof(CategoriaTransacao.DespesaBancaria)));
        Assert.IsTrue(categorias.Contains(nameof(CategoriaTransacao.PagamentoEstado)));
        Assert.IsTrue(categorias.Contains(nameof(CategoriaTransacao.Salario)));
        Assert.IsTrue(categorias.Contains(nameof(CategoriaTransacao.PagamentoCliente)));
        Assert.IsTrue(categorias.Contains(nameof(CategoriaTransacao.PagamentoFornecedor)));
        Assert.IsTrue(categorias.Contains(nameof(CategoriaTransacao.NaoClassificada)));
    }

    // ── Integração com IRegraClassificacao ───────────────────────────

    [TestMethod]
    public void RegraClassificacaoML_DeveClassificarComissao()
    {
        using var regra = new RegraClassificacaoML(_caminhoModelo, prioridade: 50, limiarConfianca: 0.3);

        var transacao = new Transacao
        {
            Id = "T1",
            Data = DateTime.Now,
            Valor = 500m,
            Descricao = "Comissão de manutenção de conta",
            Tipo = TipoTransacao.Debito
        };

        var resultado = regra.Classificar(transacao);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(CategoriaTransacao.DespesaBancaria, resultado.Categoria);
        Assert.IsTrue(resultado.Confianca > 0);
        Assert.AreEqual("ML:ClassificacaoTexto", resultado.RegraAplicada);
    }

    [TestMethod]
    public void RegraClassificacaoML_DeveClassificarPagamentoEstado()
    {
        using var regra = new RegraClassificacaoML(_caminhoModelo, prioridade: 50, limiarConfianca: 0.3);

        var transacao = new Transacao
        {
            Id = "T2",
            Data = DateTime.Now,
            Valor = 85000m,
            Descricao = "Pagamento INSS contribuições Março",
            Tipo = TipoTransacao.Debito
        };

        var resultado = regra.Classificar(transacao);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(CategoriaTransacao.PagamentoEstado, resultado.Categoria);
    }

    [TestMethod]
    public void RegraClassificacaoML_DeveClassificarSalario()
    {
        using var regra = new RegraClassificacaoML(_caminhoModelo, prioridade: 50, limiarConfianca: 0.3);

        var transacao = new Transacao
        {
            Id = "T3",
            Data = DateTime.Now,
            Valor = 650000m,
            Descricao = "Folha salarial Janeiro 2026",
            Tipo = TipoTransacao.Debito
        };

        var resultado = regra.Classificar(transacao);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(CategoriaTransacao.Salario, resultado.Categoria);
    }

    [TestMethod]
    public void RegraClassificacaoML_DeveClassificarPagamentoCliente()
    {
        using var regra = new RegraClassificacaoML(_caminhoModelo, prioridade: 50, limiarConfianca: 0.3);

        var transacao = new Transacao
        {
            Id = "T4",
            Data = DateTime.Now,
            Valor = 4400m,
            Descricao = "Deposito numerario|Depositante:JOAO CARLOS|ABC123",
            Tipo = TipoTransacao.Credito
        };

        var resultado = regra.Classificar(transacao);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(CategoriaTransacao.PagamentoCliente, resultado.Categoria);
    }

    [TestMethod]
    public void RegraClassificacaoML_DescricaoVazia_DeveRetornarNull()
    {
        using var regra = new RegraClassificacaoML(_caminhoModelo);

        var transacao = new Transacao
        {
            Id = "T5",
            Data = DateTime.Now,
            Valor = 100m,
            Descricao = "",
            Tipo = TipoTransacao.Debito
        };

        var resultado = regra.Classificar(transacao);
        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void RegraClassificacaoML_PrioridadeConfiguravel()
    {
        using var regra = new RegraClassificacaoML(_caminhoModelo, prioridade: 75);
        Assert.AreEqual(75, regra.Prioridade);
    }
}
