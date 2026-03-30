using Simansoft.BridgeBank.Core.Models;
using Simansoft.BridgeBank.ML.Reconciliacao;

namespace BridgeBank.ML.Tests;

[TestClass]
public class ReconciliacaoMLTests
{
    // ── ExtratorFeatures ─────────────────────────────────────────────

    [TestMethod]
    public void ExtrairFeatures_ReferenciaExata_DeveSerUm()
    {
        var transacao = new Transacao
        {
            Id = "T1", Data = new DateTime(2026, 1, 5),
            Valor = 5000m, Descricao = "Pagamento",
            Referencia = "INV-001", Tipo = TipoTransacao.Debito
        };
        var lancamento = new LancamentoERP
        {
            Id = "L1", Data = new DateTime(2026, 1, 5),
            Valor = 5000m, Descricao = "Factura",
            Referencia = "INV-001", Status = StatusLancamento.Pendente
        };

        var features = ExtratorFeatures.ExtrairFeatures(transacao, lancamento);

        Assert.AreEqual(1f, features.ReferenciaExata);
        Assert.AreEqual(0f, features.DiferencaValorAbsoluta, 0.01f);
        Assert.AreEqual(0f, features.DiferencaDias, 0.01f);
    }

    [TestMethod]
    public void ExtrairFeatures_ValoresDiferentes_DeveCalcularDiferenca()
    {
        var transacao = new Transacao
        {
            Id = "T1", Data = new DateTime(2026, 1, 5),
            Valor = 5000m, Descricao = "Pagamento",
            Tipo = TipoTransacao.Debito
        };
        var lancamento = new LancamentoERP
        {
            Id = "L1", Data = new DateTime(2026, 1, 7),
            Valor = 5500m, Descricao = "Factura",
            Status = StatusLancamento.Pendente
        };

        var features = ExtratorFeatures.ExtrairFeatures(transacao, lancamento);

        Assert.AreEqual(500f, features.DiferencaValorAbsoluta, 0.01f);
        Assert.AreEqual(2f, features.DiferencaDias, 0.01f);
        Assert.IsTrue(features.DiferencaValorRelativa > 0);
    }

    [TestMethod]
    public void ExtrairFeatures_DescricaoContemCliente_DeveSerUm()
    {
        var transacao = new Transacao
        {
            Id = "T1", Data = new DateTime(2026, 1, 5),
            Valor = 5000m, Descricao = "Pagamento de Vodacom Moçambique",
            Tipo = TipoTransacao.Credito
        };
        var lancamento = new LancamentoERP
        {
            Id = "L1", Data = new DateTime(2026, 1, 5),
            Valor = 5000m, Descricao = "Recebimento",
            Cliente = "Vodacom Moçambique",
            Status = StatusLancamento.Pendente
        };

        var features = ExtratorFeatures.ExtrairFeatures(transacao, lancamento);

        Assert.AreEqual(1f, features.DescricaoContemEntidade);
        Assert.AreEqual(1f, features.TipoTransacaoCoerente);
    }

    [TestMethod]
    public void ExtrairFeatures_SemReferenciasSemNomes_DevemSerZero()
    {
        var transacao = new Transacao
        {
            Id = "T1", Data = new DateTime(2026, 1, 5),
            Valor = 1000m, Descricao = "MOV 12345",
            Tipo = TipoTransacao.Debito
        };
        var lancamento = new LancamentoERP
        {
            Id = "L1", Data = new DateTime(2026, 1, 10),
            Valor = 2000m, Descricao = "Operação diversa",
            Status = StatusLancamento.Pendente
        };

        var features = ExtratorFeatures.ExtrairFeatures(transacao, lancamento);

        Assert.AreEqual(0f, features.ReferenciaExata);
        Assert.AreEqual(0f, features.ReferenciaContemSubstring);
        Assert.AreEqual(0f, features.DescricaoContemEntidade);
    }

    // ── GerarParesDeTreino ─────────────────────────────────────────

    [TestMethod]
    public void GerarParesDeTreino_DeveGerarPositivosENegativos()
    {
        var transacoes = new List<Transacao>
        {
            new() { Id = "T1", Data = new DateTime(2026, 1, 5), Valor = 5000m, Descricao = "Pag", Referencia = "REF1", Tipo = TipoTransacao.Debito }
        };
        var lancamentos = new List<LancamentoERP>
        {
            new() { Id = "L1", Data = new DateTime(2026, 1, 5), Valor = 5000m, Descricao = "Fac", Referencia = "REF1", Status = StatusLancamento.Pendente },
            new() { Id = "L2", Data = new DateTime(2026, 2, 1), Valor = 9999m, Descricao = "Outro", Status = StatusLancamento.Pendente }
        };
        var resultados = new List<ResultadoReconciliacao>
        {
            new() { Transacao = transacoes[0], LancamentoCorrespondente = lancamentos[0], TipoCorrespondencia = TipoCorrespondencia.PorReferencia }
        };

        var pares = TreinadorReconciliacao.GerarParesDeTreino(transacoes, lancamentos, resultados);

        Assert.IsTrue(pares.Count >= 2); // pelo menos 1 positivo + 1 negativo
        Assert.IsTrue(pares.Any(p => p.Correspondencia));
        Assert.IsTrue(pares.Any(p => !p.Correspondencia));
    }
}
