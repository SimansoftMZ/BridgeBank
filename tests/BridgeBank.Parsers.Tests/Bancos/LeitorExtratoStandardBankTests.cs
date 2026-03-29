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
        Assert.IsNotEmpty(extrato.NumeroConta);
    }

    [TestMethod]
    public void LerExtrato_ContaCorrectamenteIdentificada()
    {
        LeitorExtratoStandardBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Contains("5551234567", extrato.NumeroConta);
    }

    [TestMethod]
    public void LerExtrato_TransacoesExtraidas()
    {
        LeitorExtratoStandardBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.IsNotEmpty(extrato.Transacoes);

        foreach (Transacao t in extrato.Transacoes)
        {
            Assert.IsNotEmpty(t.Id);
            Assert.AreNotEqual(DateTime.MinValue, t.Data);
            Assert.IsGreaterThan(0, t.Valor, $"Valor deve ser positivo: {t.Valor}");
            Assert.IsNotEmpty(t.Descricao);
        };
    }

    [TestMethod]
    public void LerExtrato_TransacoesCreditoEDebito()
    {
        LeitorExtratoStandardBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        List<Transacao> creditos = [.. extrato.Transacoes.Where(t => t.Tipo == TipoTransacao.Credito)];
        List<Transacao> debitos = [.. extrato.Transacoes.Where(t => t.Tipo == TipoTransacao.Debito)];

        Assert.IsNotEmpty(creditos);
        Assert.IsNotEmpty(debitos);
    }

    [TestMethod]
    public void LerExtrato_DatasCorretas()
    {
        LeitorExtratoStandardBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(2025, extrato.DataInicio.Year);
        Assert.AreEqual(1, extrato.DataInicio.Month);
        Assert.IsGreaterThanOrEqualTo(extrato.DataInicio, extrato.DataFim);
    }

    [TestMethod]
    public void LerExtrato_NaoIncluiCopyright()
    {
        LeitorExtratoStandardBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.DoesNotContain(t => t.Descricao.Contains("Copyright", StringComparison.OrdinalIgnoreCase), extrato.Transacoes);
    }

    [TestMethod]
    public void SuportaArquivo_ExtensaoXlsx_RetornaTrue()
    {
        LeitorExtratoStandardBank leitor = new();
        Assert.IsTrue(leitor.SuportaArquivo("extracto.xlsx"));
    }
}