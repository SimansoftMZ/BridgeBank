using BridgeBank.Core.Models;
using BridgeBank.Parsers.Bancos;

namespace BridgeBank.Parsers.Tests.Bancos;

/// <summary>
/// Testes de integração para o leitor de extratos do BCI.
/// Requer o ficheiro de exemplo em Extractos/BCI.xls.
/// </summary>
public class LeitorExtratoBCITests
{
    private const string CaminhoFicheiro = @"C:\Users\alber\OneDrive\Ambiente de Trabalho\Extractos\BCI.xls";

    private static bool FicheiroDisponivel() => File.Exists(CaminhoFicheiro);

    [SkippableFact]
    public void LerExtrato_FicheiroReal_RetornaExtratoBCI()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto BCI não disponível");

        var leitor = new LeitorExtratoBCI();
        var extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal("BCI", extrato.Banco);
        Assert.NotEmpty(extrato.NumeroConta);
        Assert.NotEqual("N/A", extrato.NumeroConta);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_ContaCorrectamenteIdentificada()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto BCI não disponível");

        var leitor = new LeitorExtratoBCI();
        var extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal("1242812610003", extrato.NumeroConta);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_SaldoInicialCorreto()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto BCI não disponível");

        var leitor = new LeitorExtratoBCI();
        var extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal(200.00m, extrato.SaldoInicial);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_TransacoesExtraidas()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto BCI não disponível");

        var leitor = new LeitorExtratoBCI();
        var extrato = leitor.LerExtrato(CaminhoFicheiro);

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
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto BCI não disponível");

        var leitor = new LeitorExtratoBCI();
        var extrato = leitor.LerExtrato(CaminhoFicheiro);

        var primeira = extrato.Transacoes[0];
        Assert.Equal(new DateTime(2025, 12, 1), primeira.Data);
        Assert.Equal(4400.00m, primeira.Valor);
        Assert.Equal(TipoTransacao.Credito, primeira.Tipo);
        Assert.Contains("Pag. Serv.", primeira.Descricao);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_TransacoesDebitoCorretas()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto BCI não disponível");

        var leitor = new LeitorExtratoBCI();
        var extrato = leitor.LerExtrato(CaminhoFicheiro);

        // A segunda transacção é uma comissão (débito)
        var comissao = extrato.Transacoes[1];
        Assert.Equal(TipoTransacao.Debito, comissao.Tipo);
        Assert.Contains("Comissão", comissao.Descricao);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_DatasCorretas()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto BCI não disponível");

        var leitor = new LeitorExtratoBCI();
        var extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal(2025, extrato.DataInicio.Year);
        Assert.Equal(12, extrato.DataInicio.Month);
        Assert.True(extrato.DataFim >= extrato.DataInicio);
    }

    [Fact]
    public void SuportaArquivo_ExtensaoXls_RetornaTrue()
    {
        var leitor = new LeitorExtratoBCI();
        Assert.True(leitor.SuportaArquivo("extracto.xls"));
        Assert.True(leitor.SuportaArquivo("extracto.xlsx"));
    }

    [Fact]
    public void SuportaArquivo_ExtensaoCsv_RetornaFalse()
    {
        var leitor = new LeitorExtratoBCI();
        Assert.False(leitor.SuportaArquivo("extracto.csv"));
        Assert.False(leitor.SuportaArquivo("extracto.pdf"));
    }
}
