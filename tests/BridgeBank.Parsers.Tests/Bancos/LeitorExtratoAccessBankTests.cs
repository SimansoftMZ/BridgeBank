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

        Assert.AreEqual("4440005678", extrato.NumeroConta);
    }

    [TestMethod]
    public void LerExtrato_DatasCorretas()
    {
        LeitorExtratoAccessBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        // DataInicio/DataFim come from "De 01-01-2025 a 31-01-2025" header
        Assert.AreEqual(new DateTime(2025, 1, 1), extrato.DataInicio);
        Assert.AreEqual(new DateTime(2025, 1, 31), extrato.DataFim);
    }

    [TestMethod]
    public void LerExtrato_SaldosCorretos()
    {
        LeitorExtratoAccessBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(142350.00m, extrato.SaldoInicial);
        Assert.AreEqual(198725.00m, extrato.SaldoFinal);
    }

    [TestMethod]
    public void LerExtrato_TransacoesExtraidas()
    {
        LeitorExtratoAccessBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.IsNotEmpty(extrato.Transacoes);
        Assert.HasCount(10, extrato.Transacoes);
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
        Assert.AreEqual(new DateTime(2025, 1, 2), primeira.Data);
        Assert.AreEqual(38500.00m, primeira.Valor);
        Assert.AreEqual(TipoTransacao.Credito, primeira.Tipo);
        Assert.Contains("Recebimento", primeira.Descricao);
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