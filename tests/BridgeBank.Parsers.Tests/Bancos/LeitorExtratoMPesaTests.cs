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

        Assert.AreEqual("M-Pesa", extrato.Banco);
        Assert.IsNotEmpty(extrato.NumeroConta);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_CodigoCurtoCorreto()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto M-Pesa não disponível");

        LeitorExtratoMPesa leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual("901977", extrato.NumeroConta);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_DatasCorretas()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto M-Pesa não disponível");

        LeitorExtratoMPesa leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        Assert.AreEqual(new DateTime(2025, 12, 1), extrato.DataInicio.Date);
        Assert.AreEqual(new DateTime(2025, 12, 17), extrato.DataFim.Date);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_TransacoesExtraidas()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto M-Pesa não disponível");

        LeitorExtratoMPesa leitor = new();
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
    public void LerExtrato_FicheiroReal_TransacoesCreditoEDebito()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto M-Pesa não disponível");

        LeitorExtratoMPesa leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        List<Transacao> creditos = [.. extrato.Transacoes.Where(t => t.Tipo == TipoTransacao.Credito)];
        List<Transacao> debitos = [.. extrato.Transacoes.Where(t => t.Tipo == TipoTransacao.Debito)];

        Assert.IsNotEmpty(creditos);
        Assert.IsNotEmpty(debitos);
    }

    [SkippableFact]
    public void LerExtrato_FicheiroReal_ReferenciasPresentes()
    {
        Skip.IfNot(FicheiroDisponivel(), "Ficheiro de extracto M-Pesa não disponível");

        LeitorExtratoMPesa leitor = new();
        ExtratoBancario extrato = leitor.LerExtrato(CaminhoFicheiro);

        foreach (Transacao t in extrato.Transacoes)
        {
            Assert.IsNotNull(t.Referencia);
            Assert.IsNotEmpty(t.Referencia);
        }
    }

    [Fact]
    public void SuportaArquivo_ExtensaoXls_RetornaTrue()
    {
        LeitorExtratoMPesa leitor = new();
        Assert.IsTrue(leitor.SuportaArquivo("extracto.xls"));
        Assert.IsTrue(leitor.SuportaArquivo("extracto.xlsx"));
    }
}