using Simansoft.BridgeBank.Parsers.Bancos;
using Simansoft.BridgeBank.Core.Models;

namespace BridgeBank.Parsers.Tests.Bancos;

/// <summary>
/// Testes de integração para o leitor de extratos do Standard Bank.
/// Requer o ficheiro de exemplo em TestData/Extractos/StandardBank.xlsx.
/// </summary>
public class LeitorExtratoStandardBankTests
{
    private static readonly string CaminhoFicheiro =
        Path.Combine(AppContext.BaseDirectory, "TestData", "Extractos", "StandardBank.xlsx");

    private static bool FicheiroDisponivel() => File.Exists(CaminhoFicheiro);

    [SkippableFact]
    public void LerExtrato_FicheiroReal_RetornaExtratoStandardBank()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto Standard Bank não disponível");

        LeitorExtratoStandardBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal("Standard Bank", extrato.Banco);
        Assert.NotEmpty(extrato.NumeroConta);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_ContaCorrectamenteIdentificada()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto Standard Bank não disponível");

        LeitorExtratoStandardBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Contains("1046229761009", extrato.NumeroConta);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_TransacoesExtraidas()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto Standard Bank não disponível");

        LeitorExtratoStandardBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.NotEmpty(extrato.Transacoes);
        Assert.All(extrato.Transacoes, t =>
        {
            Assert.NotEmpty(t.Id);
            Assert.NotEqual(DateTime.MinValue, t.Data);
            Assert.True(t.Valor > 0, $"Valor deve ser positivo: {t.Valor}");
            Assert.NotEmpty(t.Descricao);
        });
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_PrimeiraTransacaoCorrecta()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto Standard Bank não disponível");

        LeitorExtratoStandardBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Transacao primeira = extrato.Transacoes[0];
        Assert.Equal(new DateTime(2025, 12, 15), primeira.Data);
        Assert.Equal(3180.00m, primeira.Valor);
        Assert.Equal(TipoTransacao.Credito, primeira.Tipo);
        Assert.NotNull(primeira.Referencia);
        Assert.Contains("FT253498WH0B", primeira.Referencia);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_DatasCorretas()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto Standard Bank não disponível");

        LeitorExtratoStandardBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal(2025, extrato.DataInicio.Year);
        Assert.Equal(12, extrato.DataInicio.Month);
        Assert.True(extrato.DataFim >= extrato.DataInicio);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_NaoIncluiCopyright()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto Standard Bank não disponível");

        LeitorExtratoStandardBank leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.DoesNotContain(extrato.Transacoes, t =>
            t.Descricao.Contains("Copyright", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void SuportaArquivo_ExtensaoXlsx_RetornaTrue()
    {
        LeitorExtratoStandardBank leitor = new();
        Assert.True(leitor.SuportaArquivo("extracto.xlsx"));
    }
}