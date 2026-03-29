using Simansoft.BridgeBank.Parsers.Bancos;
using Simansoft.BridgeBank.Core.Models;

namespace BridgeBank.Parsers.Tests.Bancos;

/// <summary>
/// Testes para o leitor de extratos do Millennium BIM.
/// Utiliza ficheiro fictício em Fixtures/Extractos/Millennium BIM.xlsx.
/// </summary>
[TestClass]
public class LeitorExtratoMillenniumBIMTests
{
    private static readonly string CaminhoFicheiro =
        Path.Combine(AppContext.BaseDirectory, "Fixtures", "Extractos", "Millennium BIM.xlsx");

    [TestMethod]
    public void LerExtrato_RetornaExtratoMillenniumBIM()
    {
        LeitorExtratoMillenniumBIM leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual("Millennium BIM", extrato.Banco);
        Assert.IsNotEmpty(extrato.NumeroConta);
    }

    [TestMethod]
    public void LerExtrato_ContaCorrectamenteIdentificada()
    {
        LeitorExtratoMillenniumBIM leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual("9876543210", extrato.NumeroConta);
    }

    [TestMethod]
    public void LerExtrato_DatasCorretas()
    {
        LeitorExtratoMillenniumBIM leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        // DataInicio/DataFim come from "de 01-01-2025 até 31-01-2025" header
        Assert.AreEqual(new DateTime(2025, 1, 1), extrato.DataInicio);
        Assert.AreEqual(new DateTime(2025, 1, 31), extrato.DataFim);
    }

    [TestMethod]
    public void LerExtrato_SaldosCorretos()
    {
        LeitorExtratoMillenniumBIM leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(250000.75m, extrato.SaldoInicial);
        Assert.AreEqual(394450.75m, extrato.SaldoFinal);
    }

    [TestMethod]
    public void LerExtrato_TransacoesExtraidas()
    {
        LeitorExtratoMillenniumBIM leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.IsNotEmpty(extrato.Transacoes);
        Assert.AreEqual(12, extrato.Transacoes.Count);
        foreach (Transacao t in extrato.Transacoes)
        {
            Assert.IsNotEmpty(t.Id);
            Assert.AreNotEqual(DateTime.MinValue, t.Data);
            Assert.IsGreaterThan(0, t.Valor, $"Valor deve ser positivo: {t.Valor}");
        }
    }

    [TestMethod]
    public void LerExtrato_PrimeiraTransacaoCorrecta()
    {
        LeitorExtratoMillenniumBIM leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Transacao primeira = extrato.Transacoes[0];
        Assert.AreEqual(new DateTime(2025, 1, 3), primeira.Data);
        Assert.AreEqual(35000.00m, primeira.Valor);
        Assert.AreEqual(TipoTransacao.Credito, primeira.Tipo);
        Assert.Contains("cheque", primeira.Descricao, StringComparison.OrdinalIgnoreCase);
    }

    [TestMethod]
    public void LerExtrato_TransacoesDebitoExistem()
    {
        LeitorExtratoMillenniumBIM leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        List<Transacao> debitos = [.. extrato.Transacoes.Where(t => t.Tipo == TipoTransacao.Debito)];
        Assert.IsNotEmpty(debitos);
    }

    [TestMethod]
    public void SuportaArquivo_ExtensaoXlsx_RetornaTrue()
    {
        LeitorExtratoMillenniumBIM leitor = new();
        Assert.IsTrue(leitor.SuportaArquivo("extracto.xlsx"));
    }
}
