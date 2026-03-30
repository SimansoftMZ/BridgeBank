using Simansoft.BridgeBank.Core.Classificacao;
using Simansoft.BridgeBank.Core.Classificacao.Regras;
using Simansoft.BridgeBank.Core.Models;

namespace BridgeBank.Core.Tests;

[TestClass]
public class ClassificadorTransacaoTests
{
    private ClassificadorTransacao _classificador = null!;

    [TestInitialize]
    public void Setup()
    {
        _classificador = ClassificadorTransacao.CriarComRegrasPadrao();
    }

    // --- Despesas Bancárias ---

    [TestMethod]
    public void Classificar_ComissaoBancaria_DeveRetornarDespesaBancaria()
    {
        var transacao = CriarTransacao("Comissão de manutenção de conta", -150m, TipoTransacao.Debito);

        var resultado = _classificador.Classificar(transacao);

        Assert.AreEqual(CategoriaTransacao.DespesaBancaria, resultado.Categoria);
        Assert.IsTrue(resultado.Confianca >= 0.7);
    }

    [TestMethod]
    public void Classificar_TaxaBancaria_DeveRetornarDespesaBancaria()
    {
        var transacao = CriarTransacao("Taxa de transferência SWIFT", -500m, TipoTransacao.Debito);

        var resultado = _classificador.Classificar(transacao);

        Assert.AreEqual(CategoriaTransacao.DespesaBancaria, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_JurosBancarios_DeveRetornarDespesaBancaria()
    {
        var transacao = CriarTransacao("Juros devedores do período", -1200m, TipoTransacao.Debito);

        var resultado = _classificador.Classificar(transacao);

        Assert.AreEqual(CategoriaTransacao.DespesaBancaria, resultado.Categoria);
    }

    // --- Pagamento ao Estado ---

    [TestMethod]
    public void Classificar_PagamentoINSS_DeveRetornarPagamentoEstado()
    {
        var transacao = CriarTransacao("Pagamento INSS contribuições mensais", -45000m, TipoTransacao.Debito);

        var resultado = _classificador.Classificar(transacao);

        Assert.AreEqual(CategoriaTransacao.PagamentoEstado, resultado.Categoria);
        Assert.IsTrue(resultado.Confianca >= 0.8);
    }

    [TestMethod]
    public void Classificar_PagamentoIVA_DeveRetornarPagamentoEstado()
    {
        var transacao = CriarTransacao("Pagamento IVA mês de referência", -120000m, TipoTransacao.Debito);

        var resultado = _classificador.Classificar(transacao);

        Assert.AreEqual(CategoriaTransacao.PagamentoEstado, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_PagamentoIRPC_DeveRetornarPagamentoEstado()
    {
        var transacao = CriarTransacao("IRPC - Imposto sobre rendimento", -350000m, TipoTransacao.Debito);

        var resultado = _classificador.Classificar(transacao);

        Assert.AreEqual(CategoriaTransacao.PagamentoEstado, resultado.Categoria);
    }

    // --- Salários ---

    [TestMethod]
    public void Classificar_PagamentoSalarios_DeveRetornarSalario()
    {
        var transacao = CriarTransacao("Folha salarial Março 2026", -850000m, TipoTransacao.Debito);

        var resultado = _classificador.Classificar(transacao);

        Assert.AreEqual(CategoriaTransacao.Salario, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_Vencimento_DeveRetornarSalario()
    {
        var transacao = CriarTransacao("Vencimento colaboradores", -500000m, TipoTransacao.Debito);

        var resultado = _classificador.Classificar(transacao);

        Assert.AreEqual(CategoriaTransacao.Salario, resultado.Categoria);
    }

    // --- Pagamento de Cliente ---

    [TestMethod]
    public void Classificar_RecebimentoCliente_DeveRetornarPagamentoCliente()
    {
        var transacao = CriarTransacao("Recebimento factura cliente ABC Lda", 250000m, TipoTransacao.Credito);

        var resultado = _classificador.Classificar(transacao);

        Assert.AreEqual(CategoriaTransacao.PagamentoCliente, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_DepositoCliente_DeveRetornarPagamentoCliente()
    {
        var transacao = CriarTransacao("Depósito cliente - pagamento factura", 180000m, TipoTransacao.Credito);

        var resultado = _classificador.Classificar(transacao);

        Assert.AreEqual(CategoriaTransacao.PagamentoCliente, resultado.Categoria);
    }

    // --- Pagamento a Fornecedor ---

    [TestMethod]
    public void Classificar_PagamentoFornecedor_DeveRetornarPagamentoFornecedor()
    {
        var transacao = CriarTransacao("Pagamento fornecedor XYZ materiais", -320000m, TipoTransacao.Debito);

        var resultado = _classificador.Classificar(transacao);

        Assert.AreEqual(CategoriaTransacao.PagamentoFornecedor, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_ComprasMercadoria_DeveRetornarPagamentoFornecedor()
    {
        var transacao = CriarTransacao("Compra de mercadorias escritório", -75000m, TipoTransacao.Debito);

        var resultado = _classificador.Classificar(transacao);

        Assert.AreEqual(CategoriaTransacao.PagamentoFornecedor, resultado.Categoria);
    }

    // --- Transferência Interna ---

    [TestMethod]
    public void Classificar_TransferenciaInterna_DeveRetornarTransferenciaInterna()
    {
        var transacao = CriarTransacao("Transferência interna entre contas", -500000m, TipoTransacao.Debito);

        var resultado = _classificador.Classificar(transacao);

        Assert.AreEqual(CategoriaTransacao.TransferenciaInterna, resultado.Categoria);
    }

    // --- Empréstimo ---

    [TestMethod]
    public void Classificar_PrestacaoEmprestimo_DeveRetornarEmprestimo()
    {
        var transacao = CriarTransacao("Amortização empréstimo bancário", -200000m, TipoTransacao.Debito);

        var resultado = _classificador.Classificar(transacao);

        Assert.AreEqual(CategoriaTransacao.Emprestimo, resultado.Categoria);
    }

    // --- Pagamento de Serviços ---

    [TestMethod]
    public void Classificar_PagamentoElectricidade_DeveRetornarPagamentoServicos()
    {
        var transacao = CriarTransacao("Pagamento EDM electricidade", -25000m, TipoTransacao.Debito);

        var resultado = _classificador.Classificar(transacao);

        Assert.AreEqual(CategoriaTransacao.PagamentoServicos, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_PagamentoTelecomunicacoes_DeveRetornarPagamentoServicos()
    {
        var transacao = CriarTransacao("Vodacom telecomunicações mensal", -8000m, TipoTransacao.Debito);

        var resultado = _classificador.Classificar(transacao);

        Assert.AreEqual(CategoriaTransacao.PagamentoServicos, resultado.Categoria);
    }

    // --- Não Classificada ---

    [TestMethod]
    public void Classificar_DescricaoGenerica_DeveRetornarNaoClassificada()
    {
        var transacao = CriarTransacao("Movimento ref 123456", -10000m, TipoTransacao.Debito);

        var resultado = _classificador.Classificar(transacao);

        Assert.AreEqual(CategoriaTransacao.NaoClassificada, resultado.Categoria);
        Assert.AreEqual(0, resultado.Confianca);
    }

    [TestMethod]
    public void Classificar_DescricaoVazia_DeveRetornarNaoClassificada()
    {
        var transacao = CriarTransacao("", -5000m, TipoTransacao.Debito);

        var resultado = _classificador.Classificar(transacao);

        Assert.AreEqual(CategoriaTransacao.NaoClassificada, resultado.Categoria);
    }

    // --- ClassificarEAplicar ---

    [TestMethod]
    public void ClassificarEAplicar_DevePreencherCategoriaEConfiancaNaTransacao()
    {
        var transacao = CriarTransacao("Comissão bancária mensal", -200m, TipoTransacao.Debito);

        _classificador.ClassificarEAplicar(transacao);

        Assert.AreEqual(CategoriaTransacao.DespesaBancaria, transacao.Categoria);
        Assert.IsTrue(transacao.ConfiancaClassificacao > 0);
    }

    // --- ClassificarExtrato ---

    [TestMethod]
    public void ClassificarExtrato_DeveClassificarTodasTransacoes()
    {
        var extrato = new ExtratoBancario
        {
            Banco = "BCI",
            NumeroConta = "12345",
            Transacoes =
            [
                CriarTransacao("Comissão manutenção", -100m, TipoTransacao.Debito),
                CriarTransacao("Pagamento INSS", -45000m, TipoTransacao.Debito),
                CriarTransacao("Recebimento cliente ABC", 200000m, TipoTransacao.Credito),
                CriarTransacao("Movimento genérico 999", -5000m, TipoTransacao.Debito)
            ]
        };

        var resultados = _classificador.ClassificarExtrato(extrato);

        Assert.AreEqual(4, resultados.Count);
        Assert.AreEqual(CategoriaTransacao.DespesaBancaria, extrato.Transacoes[0].Categoria);
        Assert.AreEqual(CategoriaTransacao.PagamentoEstado, extrato.Transacoes[1].Categoria);
        Assert.AreEqual(CategoriaTransacao.PagamentoCliente, extrato.Transacoes[2].Categoria);
        Assert.AreEqual(CategoriaTransacao.NaoClassificada, extrato.Transacoes[3].Categoria);
    }

    // --- Prioridade das Regras ---

    [TestMethod]
    public void Classificar_DespesaBancariaPrioridadeSuperior_QuandoConflito()
    {
        // "Taxa imposto selo" contém palavras de despesa bancária E pagamento ao Estado
        // Despesa bancária tem prioridade 95, Estado tem 90 -> deve vencer despesa bancária
        var transacao = CriarTransacao("Taxa imposto de selo", -500m, TipoTransacao.Debito);

        var resultado = _classificador.Classificar(transacao);

        Assert.AreEqual(CategoriaTransacao.DespesaBancaria, resultado.Categoria);
    }

    // --- Helper ---

    private static Transacao CriarTransacao(string descricao, decimal valor, TipoTransacao tipo)
    {
        return new Transacao
        {
            Id = Guid.NewGuid().ToString(),
            Data = new DateTime(2026, 3, 15),
            Valor = Math.Abs(valor),
            Descricao = descricao,
            Tipo = tipo
        };
    }
}
