using Simansoft.BridgeBank.Parsers.Bancos;
using Simansoft.BridgeBank.Core.Models;

namespace BridgeBank.Parsers.Tests.Bancos;

/// <summary>
/// Testes para o leitor de extratos do NedBank.
/// Utiliza ficheiro fictício em Fixtures/Extractos/NedBank.xlsx.
/// </summary>
[TestClass]
public class LeitorExtratoNedBankTests
{
    private static readonly string CaminhoFicheiro =
        Path.Combine(AppContext.BaseDirectory, "Fixtures", "Extractos", "NedBank.xlsx");

    [TestMethod]
    public void LerExtrato_RetornaExtratoNedBank()
    {
        LeitorExtratoNedBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual("NedBank", extrato.Banco);
    }

    [TestMethod]
    public void LerExtrato_SaldoAberturaCorreto()
    {
        LeitorExtratoNedBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(4302167.29m, extrato.SaldoInicial);
    }

    [TestMethod]
    public void LerExtrato_SaldoFinalCorreto()
    {
        LeitorExtratoNedBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(4664634.64m, extrato.SaldoFinal);
    }

    [TestMethod]
    public void LerExtrato_TransacoesExtraidas()
    {
        LeitorExtratoNedBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.IsNotEmpty(extrato.Transacoes);
        foreach (Transacao t in extrato.Transacoes)
        {
            Assert.IsNotEmpty(t.Id);
            Assert.AreNotEqual(DateTime.MinValue, t.Data);
            Assert.IsGreaterThan(0, t.Valor, $"Valor deve ser positivo: {t.Valor}");
        };
    }

    [TestMethod]
    public void LerExtrato_PrimeiraTransacaoCorrecta()
    {
        LeitorExtratoNedBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Transacao primeira = extrato.Transacoes[0];
        Assert.AreEqual(new DateTime(2025, 12, 1), primeira.Data);
        Assert.AreEqual(4400.00m, primeira.Valor);
        Assert.AreEqual(TipoTransacao.Credito, primeira.Tipo);
        Assert.AreEqual("IW591333NBE5", primeira.Referencia);
    }

    [TestMethod]
    public void LerExtrato_TransacoesDebitoExistem()
    {
        LeitorExtratoNedBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        List<Transacao> debitos = [.. extrato.Transacoes.Where(t => t.Tipo == TipoTransacao.Debito)];
        Assert.IsNotEmpty(debitos);
    }

    [TestMethod]
    public void LerExtrato_DatasCorretas()
    {
        LeitorExtratoNedBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(2025, extrato.DataInicio.Year);
        Assert.AreEqual(12, extrato.DataInicio.Month);
        Assert.IsGreaterThanOrEqualTo(extrato.DataInicio, extrato.DataFim);
    }

    [TestMethod]
    public void SuportaArquivo_ExtensaoXlsx_RetornaTrue()
    {
        LeitorExtratoNedBank leitor = new();
        Assert.IsTrue(leitor.SuportaArquivo("extracto.xlsx"));
    }
}
