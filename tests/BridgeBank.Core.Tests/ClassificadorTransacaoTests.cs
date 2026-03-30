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

    // ══════════════════════════════════════════════════════════════════
    // Despesas Bancárias
    // ══════════════════════════════════════════════════════════════════

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

    [TestMethod]
    public void Classificar_ComissaoPOSMillenniumBIM_DeveRetornarDespesaBancaria()
    {
        // Padrão real Millennium BIM: comissão de entidade
        var transacao = CriarTransacao("COM. ENT. 96001 17506427 928132688801 #", -250m, TipoTransacao.Debito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.DespesaBancaria, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_ComissaoTransferenciaMillenniumBIM_DeveRetornarDespesaBancaria()
    {
        // Padrão real: COM TRF INTER BALCAO
        var transacao = CriarTransacao("COM TRF INTER BALCAO INC ISRef. 960796069176041", -100m, TipoTransacao.Debito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.DespesaBancaria, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_ImpostoSeloPOS_DeveRetornarDespesaBancaria()
    {
        // Padrão real Millennium BIM
        var transacao = CriarTransacao("Imposto Selo S/ a comissao POS 107465070347 #", -15m, TipoTransacao.Debito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.DespesaBancaria, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_TxMensalNetshop_DeveRetornarDespesaBancaria()
    {
        var transacao = CriarTransacao("Tx MENSAL NETSHOP", -500m, TipoTransacao.Debito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.DespesaBancaria, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_ComVisa_DeveRetornarDespesaBancaria()
    {
        var transacao = CriarTransacao("Com VISA e IS Art 19.2.4", -350m, TipoTransacao.Debito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.DespesaBancaria, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_JurosNedBank_DeveRetornarDespesaBancaria()
    {
        // Padrão real NedBank
        var transacao = CriarTransacao("Juros -I.S-Pag Capital-Mobiliz|Capital|ZK29373LY1H4", -3500m, TipoTransacao.Debito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.DespesaBancaria, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_DevolucaoComissao_DeveRetornarDespesaBancaria()
    {
        // Padrão real Standard Bank (crédito, mas é operação bancária)
        var transacao = CriarTransacao("Regularizacao Devol.Comissao Devol.Comissao", 200m, TipoTransacao.Credito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.DespesaBancaria, resultado.Categoria);
    }

    // ══════════════════════════════════════════════════════════════════
    // Pagamento ao Estado
    // ══════════════════════════════════════════════════════════════════

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
        var transacao = CriarTransacao("IVA - Declaração periódica Fevereiro", -120000m, TipoTransacao.Debito);
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

    [TestMethod]
    public void Classificar_IRPS_DeveRetornarPagamentoEstado()
    {
        var transacao = CriarTransacao("IRPS retenção na fonte colaboradores", -45000m, TipoTransacao.Debito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.PagamentoEstado, resultado.Categoria);
    }

    // ══════════════════════════════════════════════════════════════════
    // Salários
    // ══════════════════════════════════════════════════════════════════

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

    // ══════════════════════════════════════════════════════════════════
    // Pagamento de Cliente (incluindo padrões reais)
    // ══════════════════════════════════════════════════════════════════

    [TestMethod]
    public void Classificar_RecebimentoCliente_DeveRetornarPagamentoCliente()
    {
        var transacao = CriarTransacao("Recebimento factura cliente ABC Lda", 250000m, TipoTransacao.Credito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.PagamentoCliente, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_PagServicosBCI_DeveRetornarPagamentoCliente()
    {
        // Padrão real BCI: "Pag. Serv." é um crédito de pagamento de serviços recebido
        var transacao = CriarTransacao("Pag. Serv. 12345678", 4400m, TipoTransacao.Credito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.PagamentoCliente, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_DepositoNumerarioNedBank_DeveRetornarPagamentoCliente()
    {
        // Padrão real NedBank
        var transacao = CriarTransacao("Deposito numerario|Depositante:DIANA NOELIA NHANTUMBO|IW591333NBE5", 4400m, TipoTransacao.Credito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.PagamentoCliente, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_TransferenciaRecebidaStandardBank_DeveRetornarPagamentoCliente()
    {
        // Padrão real Standard Bank
        var transacao = CriarTransacao("Trf. Recebida Internet Banking Internet Banking", 3180m, TipoTransacao.Credito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.PagamentoCliente, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_ChequeDepositadoStandardBank_DeveRetornarPagamentoCliente()
    {
        var transacao = CriarTransacao("Chq. Depositado 78658426", 15000m, TipoTransacao.Credito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.PagamentoCliente, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_OPROutroBanco_DeveRetornarPagamentoCliente()
    {
        // Padrão real Millennium BIM: operação de outro banco
        var transacao = CriarTransacao("OPR ABSA 1972715 CTZ LOGISTICOS Lda", 50000m, TipoTransacao.Credito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.PagamentoCliente, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_3rdPtyTransfer_DeveRetornarPagamentoCliente()
    {
        // Padrão real Standard Bank
        var transacao = CriarTransacao("3rdPty Transfer 2934427 2934427", 12500m, TipoTransacao.Credito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.PagamentoCliente, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_OwnerDeposit_DeveRetornarPagamentoCliente()
    {
        // Padrão real Millennium BIM
        var transacao = CriarTransacao("OWNER'S DEPOSIT DEP NUMERARIORef. 932579197454293", 8000m, TipoTransacao.Credito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.PagamentoCliente, resultado.Categoria);
    }

    // ══════════════════════════════════════════════════════════════════
    // Vendas POS / E-commerce
    // ══════════════════════════════════════════════════════════════════

    [TestMethod]
    public void Classificar_VendaPOS_DeveRetornarPagamentoCliente()
    {
        // Padrão real Millennium BIM: crédito de venda em terminal
        var transacao = CriarTransacao("POS 56361747 00918 193459618271 #", 15000m, TipoTransacao.Credito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.PagamentoCliente, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_EntidadeCreditoMillenniumBIM_DeveRetornarPagamentoCliente()
    {
        // Padrão real: "ENTIDADE 14029 CREDITO"
        var transacao = CriarTransacao("ENTIDADE 14029 CREDITO", 132829.99m, TipoTransacao.Credito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.PagamentoCliente, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_CreditoEntidadeMillenniumBIM_DeveRetornarPagamentoCliente()
    {
        // Padrão real: "CRED. ENT. 96001"
        var transacao = CriarTransacao("CRED. ENT. 96001 17506427 928132688801 #", 5000m, TipoTransacao.Credito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.PagamentoCliente, resultado.Categoria);
    }

    // ══════════════════════════════════════════════════════════════════
    // Pagamento a Fornecedor
    // ══════════════════════════════════════════════════════════════════

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

    [TestMethod]
    public void Classificar_TransferenciaFavorMillenniumBIM_DeveRetornarPagamentoFornecedor()
    {
        // Padrão real: "TRF A/F NOVATEL LDA"
        var transacao = CriarTransacao("TRF A/F NOVATEL LDA - JYU414553675036 - TRF P/O NOVATEL LDA", -25000m, TipoTransacao.Debito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.PagamentoFornecedor, resultado.Categoria);
    }

    // ══════════════════════════════════════════════════════════════════
    // Transferência Interna
    // ══════════════════════════════════════════════════════════════════

    [TestMethod]
    public void Classificar_TransferenciaInterna_DeveRetornarTransferenciaInterna()
    {
        var transacao = CriarTransacao("Transferência interna entre contas", -500000m, TipoTransacao.Debito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.TransferenciaInterna, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_TrfIntrabNedBank_DeveRetornarTransferenciaInterna()
    {
        // Padrão real NedBank
        var transacao = CriarTransacao("Trf intrab client dif balc", -100000m, TipoTransacao.Debito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.TransferenciaInterna, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_ConstituicaoDPNedBank_DeveRetornarTransferenciaInterna()
    {
        // Padrão real NedBank: constituição de depósito a prazo
        var transacao = CriarTransacao("Constituicao de DP|Capital|SV521840WVX6", -500000m, TipoTransacao.Debito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.TransferenciaInterna, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_IziTransfer_DeveRetornarTransferenciaInterna()
    {
        // Padrão real Millennium BIM
        var transacao = CriarTransacao("IZI TRANSFER P/ 32050511", -10000m, TipoTransacao.Debito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.TransferenciaInterna, resultado.Categoria);
    }

    // ══════════════════════════════════════════════════════════════════
    // Empréstimo
    // ══════════════════════════════════════════════════════════════════

    [TestMethod]
    public void Classificar_PrestacaoEmprestimo_DeveRetornarEmprestimo()
    {
        var transacao = CriarTransacao("Amortização empréstimo bancário", -200000m, TipoTransacao.Debito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.Emprestimo, resultado.Categoria);
    }

    // ══════════════════════════════════════════════════════════════════
    // Pagamento de Serviços
    // ══════════════════════════════════════════════════════════════════

    [TestMethod]
    public void Classificar_PagamentoElectricidade_DeveRetornarPagamentoServicos()
    {
        var transacao = CriarTransacao("EDM electricidade escritório", -25000m, TipoTransacao.Debito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.PagamentoServicos, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_GoHostingMillenniumBIM_DeveRetornarPagamentoServicos()
    {
        // Padrão real Millennium BIM
        var transacao = CriarTransacao("805227 GOHOSTING REDE NOVARef. 300548256814740", -8000m, TipoTransacao.Debito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.PagamentoServicos, resultado.Categoria);
    }

    // ══════════════════════════════════════════════════════════════════
    // Não Classificada
    // ══════════════════════════════════════════════════════════════════

    [TestMethod]
    public void Classificar_DescricaoGenerica_DeveRetornarNaoClassificada()
    {
        var transacao = CriarTransacao("Movimento ref 123456", -10000m, TipoTransacao.Debito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.NaoClassificada, resultado.Categoria);
    }

    [TestMethod]
    public void Classificar_DescricaoVazia_DeveRetornarNaoClassificada()
    {
        var transacao = CriarTransacao("", -5000m, TipoTransacao.Debito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.NaoClassificada, resultado.Categoria);
    }

    // ══════════════════════════════════════════════════════════════════
    // Falsos positivos (regressão)
    // ══════════════════════════════════════════════════════════════════

    [TestMethod]
    public void Classificar_CorporativaComIVA_NaoDeveSerEstado()
    {
        // "corporativa" contém "iva" como substring — não deve ser classificada como Estado
        var transacao = CriarTransacao("Vodacom internet corporativa mensal", -12000m, TipoTransacao.Debito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreNotEqual(CategoriaTransacao.PagamentoEstado, resultado.Categoria);
    }

    // ══════════════════════════════════════════════════════════════════
    // ClassificarEAplicar e ClassificarExtrato
    // ══════════════════════════════════════════════════════════════════

    [TestMethod]
    public void ClassificarEAplicar_DevePreencherCategoriaEConfiancaNaTransacao()
    {
        var transacao = CriarTransacao("Comissão bancária mensal", -200m, TipoTransacao.Debito);
        _classificador.ClassificarEAplicar(transacao);
        Assert.AreEqual(CategoriaTransacao.DespesaBancaria, transacao.Categoria);
        Assert.IsTrue(transacao.ConfiancaClassificacao > 0);
    }

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
                CriarTransacao("Deposito numerario|Depositante:JOAO|ABC123", 200000m, TipoTransacao.Credito),
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

    [TestMethod]
    public void Classificar_DespesaBancariaPrioridadeSuperior_QuandoConflito()
    {
        // "Taxa imposto de selo" contém palavras de despesa bancária E pagamento ao Estado
        // Despesa bancária tem prioridade 95, Estado tem 90
        var transacao = CriarTransacao("Taxa imposto de selo", -500m, TipoTransacao.Debito);
        var resultado = _classificador.Classificar(transacao);
        Assert.AreEqual(CategoriaTransacao.DespesaBancaria, resultado.Categoria);
    }

    // ── Helper ────────────────────────────────────────────────────────

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
