using BridgeBank.Core.Models;
using BridgeBank.Parsers.Bancos;

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

        var leitor = new LeitorExtratoNedBank();
        var extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal("NedBank", extrato.Banco);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_SaldoAberturaCorreto()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto NedBank não disponível");

        var leitor = new LeitorExtratoNedBank();
        var extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal(4302167.29m, extrato.SaldoInicial);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_SaldoFinalCorreto()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto NedBank não disponível");

        var leitor = new LeitorExtratoNedBank();
        var extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal(4664634.64m, extrato.SaldoFinal);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_TransacoesExtraidas()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto NedBank não disponível");

        var leitor = new LeitorExtratoNedBank();
        var extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.NotEmpty(extrato.Transacoes);
        Assert.All(extrato.Transacoes, t =>
        {
            Assert.NotEmpty(t.Id);
            Assert.NotEqual(DateTime.MinValue, t.Data);
            Assert.True(t.Valor > 0, $"Valor deve ser positivo: {t.Valor}");
        });
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_PrimeiraTransacaoCorrecta()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto NedBank não disponível");

        var leitor = new LeitorExtratoNedBank();
        var extrato = leitor.LerExtrato(CaminhoFicheiro);

        var primeira = extrato.Transacoes[0];
        Assert.Equal(new DateTime(2025, 12, 1), primeira.Data);
        Assert.Equal(4400.00m, primeira.Valor);
        Assert.Equal(TipoTransacao.Credito, primeira.Tipo);
        Assert.Equal("TT253350YHJ6", primeira.Referencia);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_TransacoesDebitoExistem()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto NedBank não disponível");

        var leitor = new LeitorExtratoNedBank();
        var extrato = leitor.LerExtrato(CaminhoFicheiro);

        var debitos = extrato.Transacoes.Where(t => t.Tipo == TipoTransacao.Debito).ToList();
        Assert.NotEmpty(debitos);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_DatasCorretas()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto NedBank não disponível");

        var leitor = new LeitorExtratoNedBank();
        var extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal(2025, extrato.DataInicio.Year);
        Assert.Equal(12, extrato.DataInicio.Month);
        Assert.True(extrato.DataFim >= extrato.DataInicio);
    }

    [Fact]
    public void SuportaArquivo_ExtensaoXlsx_RetornaTrue()
    {
        var leitor = new LeitorExtratoNedBank();
        Assert.True(leitor.SuportaArquivo("extracto.xlsx"));
    }
}
