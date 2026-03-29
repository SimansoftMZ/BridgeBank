using Simansoft.BridgeBank.Parsers.Bancos;
using Simansoft.BridgeBank.Core.Models;

namespace BridgeBank.Parsers.Tests.Bancos;

/// <summary>
/// Testes para o leitor de extratos do BCI.
/// Utiliza ficheiro fictício em Fixtures/Extractos/BCI.xls.
/// </summary>
[TestClass]
public class LeitorExtratoBCITests
{
    private static readonly string CaminhoFicheiro =
        Path.Combine(AppContext.BaseDirectory, "Fixtures", "Extractos", "BCI.xls");

    [TestMethod]
    public void LerExtrato_RetornaExtratoBCI()
    {
        LeitorExtratoBCI leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual("BCI", extrato.Banco);
        Assert.IsNotEmpty(extrato.NumeroConta);
        Assert.AreNotEqual("N/A", extrato.NumeroConta);
    }

    [TestMethod]
    public void LerExtrato_ContaCorrectamenteIdentificada()
    {
        LeitorExtratoBCI leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual("1234567890", extrato.NumeroConta);
    }

    [TestMethod]
    public void LerExtrato_SaldoInicialCorreto()
    {
        LeitorExtratoBCI leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(125000.50m, extrato.SaldoInicial);
    }

    [TestMethod]
    public void LerExtrato_TransacoesExtraidas()
    {
        LeitorExtratoBCI leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.IsNotEmpty(extrato.Transacoes);
        Assert.HasCount(10, extrato.Transacoes);
        foreach (Transacao t in extrato.Transacoes)
        {
            Assert.IsNotEmpty(t.Id);
            Assert.AreNotEqual(DateTime.MinValue, t.Data);
            Assert.IsGreaterThan(0, t.Valor, $"Valor deve ser positivo: {t.Valor}");
            Assert.IsNotEmpty(t.Descricao);
        }
    }

    [TestMethod]
    public void LerExtrato_PrimeiraTransacaoCorrecta()
    {
        LeitorExtratoBCI leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Transacao primeira = extrato.Transacoes[0];
        Assert.AreEqual(new DateTime(2025, 1, 2), primeira.Data);
        Assert.AreEqual(15750.00m, primeira.Valor);
        Assert.AreEqual(TipoTransacao.Debito, primeira.Tipo);
        Assert.Contains("fornecedor", primeira.Descricao);
    }

    [TestMethod]
    public void LerExtrato_TransacoesCreditoExistem()
    {
        LeitorExtratoBCI leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        List<Transacao> creditos = [.. extrato.Transacoes.Where(t => t.Tipo == TipoTransacao.Credito)];
        Assert.IsNotEmpty(creditos);
        // Segunda transacao e um credito
        Assert.AreEqual(TipoTransacao.Credito, extrato.Transacoes[1].Tipo);
        Assert.AreEqual(45000.00m, extrato.Transacoes[1].Valor);
    }

    [TestMethod]
    public void LerExtrato_DatasCorretas()
    {
        LeitorExtratoBCI leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(2025, extrato.DataInicio.Year);
        Assert.AreEqual(1, extrato.DataInicio.Month);
        Assert.IsGreaterThanOrEqualTo(extrato.DataInicio, extrato.DataFim);
    }

    [TestMethod]
    public void LerExtrato_SaldoFinalCorreto()
    {
        LeitorExtratoBCI leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(208000.25m, extrato.SaldoFinal);
    }

    [TestMethod]
    public void SuportaArquivo_ExtensaoXls_RetornaTrue()
    {
        LeitorExtratoBCI leitor = new();
        Assert.IsTrue(leitor.SuportaArquivo("extracto.xls"));
        Assert.IsTrue(leitor.SuportaArquivo("extracto.xlsx"));
    }

    [TestMethod]
    public void SuportaArquivo_ExtensaoCsv_RetornaFalse()
    {
        LeitorExtratoBCI leitor = new();
        Assert.IsFalse(leitor.SuportaArquivo("extracto.csv"));
        Assert.IsFalse(leitor.SuportaArquivo("extracto.pdf"));
    }
}
