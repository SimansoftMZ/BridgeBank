using BridgeBank.Core.Models;
using BridgeBank.Parsers.Bancos;

namespace BridgeBank.Parsers.Tests.Bancos;

/// <summary>
/// Testes de integração para o leitor de extratos do Access Bank.
/// Requer o ficheiro de exemplo em Extractos/Access Bank.xls.
/// </summary>
public class LeitorExtratoAccessBankTests
{
    private const string CaminhoFicheiro = @"C:\Users\alber\OneDrive\Ambiente de Trabalho\Extractos\Access Bank.xls";

    private static bool FicheiroDisponivel() => File.Exists(CaminhoFicheiro);

    [SkippableFact]
    public void LerExtrato_FicheiroReal_RetornaExtratoAccessBank()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto Access Bank não disponível");

        var leitor = new LeitorExtratoAccessBank();
        var extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal("Access Bank", extrato.Banco);
        Assert.NotEmpty(extrato.NumeroConta);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_ContaCorrectamenteIdentificada()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto Access Bank não disponível");

        var leitor = new LeitorExtratoAccessBank();
        var extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal("00101010001133", extrato.NumeroConta);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_DatasCorretas()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto Access Bank não disponível");

        var leitor = new LeitorExtratoAccessBank();
        var extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal(new DateTime(2025, 12, 1), extrato.DataInicio);
        Assert.Equal(new DateTime(2025, 12, 17), extrato.DataFim);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_SaldosCorretos()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto Access Bank não disponível");

        var leitor = new LeitorExtratoAccessBank();
        var extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal(5488800.44m, extrato.SaldoInicial);
        Assert.Equal(5299234.28m, extrato.SaldoFinal);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_TransacoesExtraidas()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto Access Bank não disponível");

        var leitor = new LeitorExtratoAccessBank();
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
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto Access Bank não disponível");

        var leitor = new LeitorExtratoAccessBank();
        var extrato = leitor.LerExtrato(CaminhoFicheiro);

        var primeira = extrato.Transacoes[0];
        Assert.Equal(new DateTime(2025, 12, 1), primeira.Data);
        Assert.Equal(4400.00m, primeira.Valor);
        Assert.Equal(TipoTransacao.Credito, primeira.Tipo);
        Assert.Contains("DEPÓSITO", primeira.Descricao);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_TransacoesDebitoExistem()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto Access Bank não disponível");

        var leitor = new LeitorExtratoAccessBank();
        var extrato = leitor.LerExtrato(CaminhoFicheiro);

        var debitos = extrato.Transacoes.Where(t => t.Tipo == TipoTransacao.Debito).ToList();
        Assert.NotEmpty(debitos);
        Assert.All(debitos, d => Assert.Contains("TRANSFERENCIA", d.Descricao));
    }

    [Fact]
    public void SuportaArquivo_ExtensaoXls_RetornaTrue()
    {
        var leitor = new LeitorExtratoAccessBank();
        Assert.True(leitor.SuportaArquivo("extracto.xls"));
        Assert.True(leitor.SuportaArquivo("extracto.xlsx"));
    }
}
