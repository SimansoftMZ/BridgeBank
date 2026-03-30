using Simansoft.BridgeBank.Parsers.Bancos;
using Simansoft.BridgeBank.Core.Models;

namespace BridgeBank.Parsers.Tests.Bancos;

/// <summary>
/// Testes para o leitor de extratos do Access Bank.
/// Utiliza ficheiro fictício em Fixtures/Extractos/Access Bank.xls.
/// </summary>
[TestClass]
public class LeitorExtratoAccessBankTests
{
    private static readonly string CaminhoFicheiro =
        Path.Combine(AppContext.BaseDirectory, "Fixtures", "Extractos", "Access Bank.xls");

    [TestMethod]
    public void LerExtrato_RetornaExtratoAccessBank()
    {
        LeitorExtratoAccessBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual("Access Bank", extrato.Banco);
        Assert.IsNotEmpty(extrato.NumeroConta);
    }

    [TestMethod]
    public void LerExtrato_ContaCorrectamenteIdentificada()
    {
        LeitorExtratoAccessBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual("50536860631688", extrato.NumeroConta);
    }

    [TestMethod]
    public void LerExtrato_DatasCorretas()
    {
        LeitorExtratoAccessBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(new DateTime(2025, 12, 1), extrato.DataInicio);
        Assert.AreEqual(new DateTime(2025, 12, 17), extrato.DataFim);
    }

    [TestMethod]
    public void LerExtrato_SaldosCorretos()
    {
        LeitorExtratoAccessBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(5488800.44m, extrato.SaldoInicial);
        Assert.AreEqual(5299234.28m, extrato.SaldoFinal);
    }

    [TestMethod]
    public void LerExtrato_TransacoesExtraidas()
    {
        LeitorExtratoAccessBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.IsNotEmpty(extrato.Transacoes);
        foreach (Transacao t in extrato.Transacoes)
        {
            Assert.IsFalse(string.IsNullOrEmpty(t.Id));
            Assert.AreNotEqual(DateTime.MinValue, t.Data);
            Assert.IsGreaterThan(0, t.Valor, $"Valor deve ser positivo: {t.Valor}");
            Assert.IsFalse(string.IsNullOrEmpty(t.Descricao));
        }
    }

    [TestMethod]
    public void LerExtrato_PrimeiraTransacaoCorrecta()
    {
        LeitorExtratoAccessBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Transacao primeira = extrato.Transacoes[0];
        Assert.AreEqual(new DateTime(2025, 12, 1), primeira.Data);
        Assert.AreEqual(4400.00m, primeira.Valor);
        Assert.AreEqual(TipoTransacao.Credito, primeira.Tipo);
        Assert.Contains("DEPÓSITO", primeira.Descricao);
    }

    [TestMethod]
    public void LerExtrato_TransacoesDebitoExistem()
    {
        LeitorExtratoAccessBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        List<Transacao> debitos = [.. extrato.Transacoes.Where(t => t.Tipo == TipoTransacao.Debito)];
        Assert.IsNotEmpty(debitos);
    }

    [TestMethod]
    public void SuportaArquivo_ExtensaoXls_RetornaTrue()
    {
        LeitorExtratoAccessBank leitor = new();
        Assert.IsTrue(leitor.SuportaArquivo("extracto.xls"));
        Assert.IsTrue(leitor.SuportaArquivo("extracto.xlsx"));
    }
}
