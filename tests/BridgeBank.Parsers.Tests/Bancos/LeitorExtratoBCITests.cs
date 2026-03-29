using Simansoft.BridgeBank.Parsers.Bancos;
using Simansoft.BridgeBank.Core.Models;

namespace BridgeBank.Parsers.Tests.Bancos;

/// <summary>
/// Testes de integração para o leitor de extratos do BCI.
/// Requer o ficheiro de exemplo em TestData/Extractos/BCI.xls.
/// </summary>
public class LeitorExtratoBCITests
{
    private static readonly string CaminhoFicheiro =
        Path.Combine(AppContext.BaseDirectory, "TestData", "Extractos", "BCI.xls");

    private static bool FicheiroDisponivel() => File.Exists(CaminhoFicheiro);

    [SkippableFact]
    public void LerExtrato_FicheiroReal_RetornaExtratoBCI()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto BCI não disponível");

        LeitorExtratoBCI leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual("BCI", extrato.Banco);
        Assert.IsNotEmpty(extrato.NumeroConta);
        Assert.AreNotEqual("N/A", extrato.NumeroConta);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_ContaCorrectamenteIdentificada()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto BCI não disponível");

        LeitorExtratoBCI leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual("1242812610003", extrato.NumeroConta);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_SaldoInicialCorreto()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto BCI não disponível");

        LeitorExtratoBCI leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(200.00m, extrato.SaldoInicial);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_TransacoesExtraidas()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto BCI não disponível");

        LeitorExtratoBCI leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.IsNotEmpty(extrato.Transacoes);
        foreach (Transacao t in extrato.Transacoes)
        {
            Assert.IsNotEmpty(t.Id);
            Assert.AreNotEqual(DateTime.MinValue, t.Data);
            Assert.IsGreaterThan(0, t.Valor, $"Valor deve ser positivo: {t.Valor}");
            Assert.IsNotEmpty(t.Descricao);
        }
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_PrimeiraTransacaoCorrecta()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto BCI não disponível");

        LeitorExtratoBCI leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Transacao primeira = extrato.Transacoes[0];
        Assert.AreEqual(new DateTime(2025, 12, 1), primeira.Data);
        Assert.AreEqual(4400.00m, primeira.Valor);
        Assert.AreEqual(TipoTransacao.Credito, primeira.Tipo);
        Assert.Contains("Pag. Serv.", primeira.Descricao);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_TransacoesDebitoCorretas()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto BCI não disponível");

        LeitorExtratoBCI leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        // A segunda transacção é uma comissão (débito)
        Transacao comissao = extrato.Transacoes[1];
        Assert.AreEqual(TipoTransacao.Debito, comissao.Tipo);
        Assert.Contains("Comissão", comissao.Descricao);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_DatasCorretas()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto BCI não disponível");

        LeitorExtratoBCI leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(2025, extrato.DataInicio.Year);
        Assert.AreEqual(12, extrato.DataInicio.Month);
        Assert.IsGreaterThanOrEqualTo(extrato.DataInicio, extrato.DataFim);
    }

    [Fact]
    public void SuportaArquivo_ExtensaoXls_RetornaTrue()
    {
        LeitorExtratoBCI leitor = new();
        Assert.IsTrue(leitor.SuportaArquivo("extracto.xls"));
        Assert.IsTrue(leitor.SuportaArquivo("extracto.xlsx"));
    }

    [Fact]
    public void SuportaArquivo_ExtensaoCsv_RetornaFalse()
    {
        LeitorExtratoBCI leitor = new();
        Assert.IsFalse(leitor.SuportaArquivo("extracto.csv"));
        Assert.IsFalse(leitor.SuportaArquivo("extracto.pdf"));
    }
}