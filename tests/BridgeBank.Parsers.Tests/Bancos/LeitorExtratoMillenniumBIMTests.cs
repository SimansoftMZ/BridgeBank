using Simansoft.BridgeBank.Parsers.Bancos;
using Simansoft.BridgeBank.Core.Models;

namespace BridgeBank.Parsers.Tests.Bancos;

/// <summary>
/// Testes de integração para o leitor de extratos do Millennium BIM.
/// Requer o ficheiro de exemplo em TestData/Extractos/Millennium BIM.xlsx.
/// </summary>
public class LeitorExtratoMillenniumBIMTests
{
    private static readonly string CaminhoFicheiro =
        Path.Combine(AppContext.BaseDirectory, "TestData", "Extractos", "Millennium BIM.xlsx");

    private static bool FicheiroDisponivel() => File.Exists(CaminhoFicheiro);

    [SkippableFact]
    public void LerExtrato_FicheiroReal_RetornaExtratoMillenniumBIM()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto Millennium BIM não disponível");

        LeitorExtratoMillenniumBIM leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal("Millennium BIM", extrato.Banco);
        Assert.NotEmpty(extrato.NumeroConta);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_ContaCorrectamenteIdentificada()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto Millennium BIM não disponível");

        LeitorExtratoMillenniumBIM leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal("125986123", extrato.NumeroConta);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_DatasCorretas()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto Millennium BIM não disponível");

        LeitorExtratoMillenniumBIM leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal(new DateTime(2025, 12, 1), extrato.DataInicio);
        Assert.Equal(new DateTime(2025, 12, 17), extrato.DataFim);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_SaldosCorretos()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto Millennium BIM não disponível");

        LeitorExtratoMillenniumBIM leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal(8965395.17m, extrato.SaldoInicial);
        Assert.Equal(3371027.03m, extrato.SaldoFinal);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_TransacoesExtraidas()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto Millennium BIM não disponível");

        LeitorExtratoMillenniumBIM leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

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
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto Millennium BIM não disponível");

        LeitorExtratoMillenniumBIM leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Transacao primeira = extrato.Transacoes[0];
        Assert.Equal(new DateTime(2025, 12, 16), primeira.Data);
        Assert.Equal(132829.99m, primeira.Valor);
        Assert.Equal(TipoTransacao.Credito, primeira.Tipo);
        Assert.Contains("ENTIDADE 96001 CREDITO", primeira.Descricao);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_TransacoesDebitoExistem()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto Millennium BIM não disponível");

        LeitorExtratoMillenniumBIM leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        List<Transacao> debitos = extrato.Transacoes.Where(t => t.Tipo == TipoTransacao.Debito).ToList();
        Assert.NotEmpty(debitos);
    }

    [Fact]
    public void SuportaArquivo_ExtensaoXlsx_RetornaTrue()
    {
        LeitorExtratoMillenniumBIM leitor = new();
        Assert.True(leitor.SuportaArquivo("extracto.xlsx"));
    }
}
