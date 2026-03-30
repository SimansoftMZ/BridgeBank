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

        Assert.AreEqual("604130508", extrato.NumeroConta);
    }

    [TestMethod]
    public void LerExtrato_DatasCorretas()
    {
        LeitorExtratoMillenniumBIM leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(new DateTime(2025, 12, 1), extrato.DataInicio);
        Assert.AreEqual(new DateTime(2025, 12, 17), extrato.DataFim);
    }

    [TestMethod]
    public void LerExtrato_SaldosCorretos()
    {
        LeitorExtratoMillenniumBIM leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(8965395.17m, extrato.SaldoInicial);
        Assert.AreEqual(3371027.03m, extrato.SaldoFinal);
    }

    [TestMethod]
    public void LerExtrato_TransacoesExtraidas()
    {
        LeitorExtratoMillenniumBIM leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.IsNotEmpty(extrato.Transacoes);
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
        Assert.AreEqual(new DateTime(2025, 12, 16), primeira.Data);
        Assert.AreEqual(132829.99m, primeira.Valor);
        Assert.AreEqual(TipoTransacao.Credito, primeira.Tipo);
        Assert.Contains("ENTIDADE 14029 CREDITO", primeira.Descricao);
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
