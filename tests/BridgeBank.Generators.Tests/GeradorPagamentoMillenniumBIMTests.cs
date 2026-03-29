using BridgeBank.Generators.Bancos;
using Simansoft.BridgeBank.Generators.Models;

namespace BridgeBank.Generators.Tests;

public class GeradorPagamentoMillenniumBIMTests
{
    [Fact]
    public void GerarFicheiro_DeveGerarCsvComCabecalho()
    {
        // Arrange
        var gerador = new GeradorPagamentoMillenniumBIM();
        var pagamentos = new List<Pagamento>
        {
            new Pagamento
            {
                Id = "P1",
                DataPagamento = new DateTime(2026, 1, 1),
                Valor = 1000.50m,
                Beneficiario = "Fornecedor A",
                ContaBeneficiario = "123456789",
                BancoBeneficiario = "BCI",
                Referencia = "REF-001"
            }
        };

        var caminhoTemp = Path.GetTempFileName();

        // Act
        gerador.GerarFicheiro(pagamentos, caminhoTemp);

        // Assert
        var linhas = File.ReadAllLines(caminhoTemp);
        Assert.Equal(2, linhas.Length);
        Assert.Contains("Data;Beneficiario", linhas[0]);
        Assert.Contains("Fornecedor A", linhas[1]);

        File.Delete(caminhoTemp);
    }
}
