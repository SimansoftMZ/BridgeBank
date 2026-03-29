using Simansoft.BridgeBank.Parsers.Bancos;
using Simansoft.BridgeBank.Core.Models;

namespace BridgeBank.Parsers.Tests.Bancos;

/// <summary>
/// Testes para o leitor de extratos do M-Pesa.
/// Utiliza ficheiro fictício em Fixtures/Extractos/M-Pesa.xls.
/// </summary>
[TestClass]
public class LeitorExtratoMPesaTests
{
    private static readonly string CaminhoFicheiro =
        Path.Combine(AppContext.BaseDirectory, "Fixtures", "Extractos", "M-Pesa.xls");

    [TestMethod]
    public void LerExtrato_RetornaExtratoMPesa()
    {
        LeitorExtratoMPesa leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual("M-Pesa", extrato.Banco);
        Assert.IsNotEmpty(extrato.NumeroConta);
    }

    [TestMethod]
    public void LerExtrato_CodigoCurtoCorreto()
    {
        LeitorExtratoMPesa leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual("258841234567", extrato.NumeroConta);
    }

    [TestMethod]
    public void LerExtrato_DatasCorretas()
    {
        LeitorExtratoMPesa leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(2025, extrato.DataInicio.Year);
        Assert.AreEqual(1, extrato.DataInicio.Month);
    }

    [TestMethod]
    public void LerExtrato_TransacoesExtraidas()
    {
        LeitorExtratoMPesa leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.IsNotEmpty(extrato.Transacoes);
        // 13 completed transactions (1 reversed is excluded)
        foreach (Transacao t in extrato.Transacoes)
        {
            Assert.IsNotEmpty(t.Id);
            Assert.AreNotEqual(DateTime.MinValue, t.Data);
            Assert.IsGreaterThan(0, t.Valor, $"Valor deve ser positivo: {t.Valor}");
        };
    }

    [TestMethod]
    public void LerExtrato_TransacoesCreditoEDebito()
    {
        LeitorExtratoMPesa leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        List<Transacao> creditos = [.. extrato.Transacoes.Where(t => t.Tipo == TipoTransacao.Credito)];
        List<Transacao> debitos = [.. extrato.Transacoes.Where(t => t.Tipo == TipoTransacao.Debito)];

        Assert.IsNotEmpty(creditos);
        Assert.IsNotEmpty(debitos);
    }

    [TestMethod]
    public void LerExtrato_ReferenciasPresentes()
    {
        LeitorExtratoMPesa leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        foreach (Transacao t in extrato.Transacoes)
        {
            Assert.IsNotNull(t.Referencia);
            Assert.IsNotEmpty(t.Referencia);
        }
    }

    [TestMethod]
    public void LerExtrato_TransacaoRevertidaExcluida()
    {
        LeitorExtratoMPesa leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        // A transacao com status "Reversed" nao deve ser incluida
        Assert.DoesNotContain(t => t.Referencia == "RCP0001014", extrato.Transacoes);
    }

    [TestMethod]
    public void SuportaArquivo_ExtensaoXls_RetornaTrue()
    {
        LeitorExtratoMPesa leitor = new();
        Assert.IsTrue(leitor.SuportaArquivo("extracto.xls"));
        Assert.IsTrue(leitor.SuportaArquivo("extracto.xlsx"));
    }
}
