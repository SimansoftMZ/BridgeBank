using Simansoft.BridgeBank.Parsers.Bancos;
using Simansoft.BridgeBank.Core.Models;

namespace BridgeBank.Parsers.Tests.Bancos;

/// <summary>
/// Testes para o leitor de extratos do Standard Bank.
/// Utiliza ficheiro fictício em Fixtures/Extractos/StandardBank.xlsx.
/// </summary>
[TestClass]
public class LeitorExtratoStandardBankTests
{
    private static readonly string CaminhoFicheiro =
        Path.Combine(AppContext.BaseDirectory, "Fixtures", "Extractos", "StandardBank.xlsx");

    [TestMethod]
    public void LerExtrato_RetornaExtratoStandardBank()
    {
        LeitorExtratoStandardBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual("Standard Bank", extrato.Banco);
        Assert.IsFalse(string.IsNullOrEmpty(extrato.NumeroConta));
    }

    [TestMethod]
    public void LerExtrato_ContaCorrectamenteIdentificada()
    {
        LeitorExtratoStandardBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        StringAssert.Contains(extrato.NumeroConta, "7492092919320");
    }

    [TestMethod]
    public void LerExtrato_TransacoesExtraidas()
    {
        LeitorExtratoStandardBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.IsTrue(extrato.Transacoes.Count > 0);

        foreach (Transacao t in extrato.Transacoes)
        {
            Assert.IsFalse(string.IsNullOrEmpty(t.Id));
            Assert.AreNotEqual(DateTime.MinValue, t.Data);
            Assert.IsTrue(t.Valor > 0, $"Valor deve ser positivo: {t.Valor}");
            Assert.IsFalse(string.IsNullOrEmpty(t.Descricao));
        };
    }

    [TestMethod]
    public void LerExtrato_PrimeiraTransacaoCorrecta()
    {
        LeitorExtratoStandardBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Transacao primeira = extrato.Transacoes[0];
        Assert.AreEqual(new DateTime(2025, 12, 15), primeira.Data);
        Assert.AreEqual(3180.00m, primeira.Valor);
        Assert.AreEqual(TipoTransacao.Credito, primeira.Tipo);
        Assert.IsNotNull(primeira.Referencia);
    }

    [TestMethod]
    public void LerExtrato_DatasCorretas()
    {
        LeitorExtratoStandardBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(2025, extrato.DataInicio.Year);
        Assert.AreEqual(12, extrato.DataInicio.Month);
        Assert.IsTrue(extrato.DataFim >= extrato.DataInicio);
    }

    [TestMethod]
    public void LerExtrato_NaoIncluiCopyright()
    {
        LeitorExtratoStandardBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.IsFalse(extrato.Transacoes.Any(t => t.Descricao.Contains("Copyright", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void SuportaArquivo_ExtensaoXlsx_RetornaTrue()
    {
        LeitorExtratoStandardBank leitor = new();
        Assert.IsTrue(leitor.SuportaArquivo("extracto.xlsx"));
    }
}
