using Simansoft.BridgeBank.Parsers.Bancos;
using Simansoft.BridgeBank.Core.Models;

namespace BridgeBank.Parsers.Tests.Bancos;

/// <summary>
/// Testes de integração para o leitor de extratos do NedBank.
/// Requer o ficheiro de exemplo em TestData/Extractos/NedBank.xlsx.
/// </summary>
public class LeitorExtratoNedBankTests
{
    private static readonly string CaminhoFicheiro =
        Path.Combine(AppContext.BaseDirectory, "TestData", "Extractos", "NedBank.xlsx");

    private static bool FicheiroDisponivel() => File.Exists(CaminhoFicheiro);

    [SkippableFact]
    public void LerExtrato_FicheiroReal_RetornaExtratoNedBank()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto NedBank não disponível");

        LeitorExtratoNedBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual("NedBank", extrato.Banco);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_SaldoAberturaCorreto()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto NedBank não disponível");

        LeitorExtratoNedBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(4302167.29m, extrato.SaldoInicial);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_SaldoFinalCorreto()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto NedBank não disponível");

        LeitorExtratoNedBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(4664634.64m, extrato.SaldoFinal);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_TransacoesExtraidas()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto NedBank não disponível");

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

    [SkippableFact]
    public void LerExtrato_FicheiroReal_PrimeiraTransacaoCorrecta()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto NedBank não disponível");

        LeitorExtratoNedBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Transacao primeira = extrato.Transacoes[0];
        Assert.AreEqual(new DateTime(2025, 12, 1), primeira.Data);
        Assert.AreEqual(4400.00m, primeira.Valor);
        Assert.AreEqual(TipoTransacao.Credito, primeira.Tipo);
        Assert.AreEqual("TT253350YHJ6", primeira.Referencia);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_TransacoesDebitoExistem()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto NedBank não disponível");

        LeitorExtratoNedBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        List<Transacao> debitos = [.. extrato.Transacoes.Where(t => t.Tipo == TipoTransacao.Debito)];
        Assert.IsNotEmpty(debitos);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_DatasCorretas()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto NedBank não disponível");

        LeitorExtratoNedBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(2025, extrato.DataInicio.Year);
        Assert.AreEqual(12, extrato.DataInicio.Month);
        Assert.IsGreaterThanOrEqualTo(extrato.DataInicio, extrato.DataFim);
    }

    [Fact]
    public void SuportaArquivo_ExtensaoXlsx_RetornaTrue()
    {
        LeitorExtratoNedBank leitor = new();
        Assert.IsTrue(leitor.SuportaArquivo("extracto.xlsx"));
    }
}
