using Simansoft.BridgeBank.Parsers.Bancos;
using Simansoft.BridgeBank.Core.Models;

namespace BridgeBank.Parsers.Tests.Bancos;

/// <summary>
/// Testes de integração para o leitor de extratos do M-Pesa.
/// Requer o ficheiro de exemplo em TestData/Extractos/M-Pesa.xls.
/// </summary>
public class LeitorExtratoMPesaTests
{
    private static readonly string CaminhoFicheiro =
        Path.Combine(AppContext.BaseDirectory, "TestData", "Extractos", "M-Pesa.xls");

    private static bool FicheiroDisponivel() => File.Exists(CaminhoFicheiro);

    [SkippableFact]
    public void LerExtrato_FicheiroReal_RetornaExtratoMPesa()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto M-Pesa não disponível");

        LeitorExtratoMPesa leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal("M-Pesa", extrato.Banco);
        Assert.NotEmpty(extrato.NumeroConta);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_CodigoCurtoCorreto()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto M-Pesa não disponível");

        LeitorExtratoMPesa leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal("901977", extrato.NumeroConta);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_DatasCorretas()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto M-Pesa não disponível");

        LeitorExtratoMPesa leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.Equal(new DateTime(2025, 12, 1), extrato.DataInicio.Date);
        Assert.Equal(new DateTime(2025, 12, 17), extrato.DataFim.Date);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_TransacoesExtraidas()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto M-Pesa não disponível");

        LeitorExtratoMPesa leitor = new();
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
    public void LerExtrato_FicheiroReal_TransacoesCreditoEDebito()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto M-Pesa não disponível");

        LeitorExtratoMPesa leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        List<Transacao> creditos = extrato.Transacoes.Where(t => t.Tipo == TipoTransacao.Credito).ToList();
        List<Transacao> debitos = extrato.Transacoes.Where(t => t.Tipo == TipoTransacao.Debito).ToList();

        Assert.NotEmpty(creditos);
        Assert.NotEmpty(debitos);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_ReferenciasPresentes()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto M-Pesa não disponível");

        LeitorExtratoMPesa leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.All(extrato.Transacoes, t =>
        {
            Assert.NotNull(t.Referencia);
            Assert.NotEmpty(t.Referencia);
        });
    }

    [Fact]
    public void SuportaArquivo_ExtensaoXls_RetornaTrue()
    {
        LeitorExtratoMPesa leitor = new();
        Assert.True(leitor.SuportaArquivo("extracto.xls"));
        Assert.True(leitor.SuportaArquivo("extracto.xlsx"));
    }
}