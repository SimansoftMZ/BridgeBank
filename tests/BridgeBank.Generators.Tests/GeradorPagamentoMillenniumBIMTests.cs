using Simansoft.BridgeBank.Generators.Bancos;
using Simansoft.BridgeBank.Generators.Models;

namespace BridgeBank.Generators.Tests;

public class GeradorPagamentoMillenniumBIMTests
{
    [Fact]
    public void GerarFicheiro_DeveGerarCsvComCabecalho()
    {
        // Arrange
        GeradorPagamentoMillenniumBIM gerador = new();
        List<Pagamento> pagamentos =
        [
            new() {
                Id = "P1",
                DataPagamento = new DateTime(2026, 1, 1),
                Valor = 1000.50m,
                Beneficiario = "Fornecedor A",
                ContaBeneficiario = "123456789",
                BancoBeneficiario = "BCI",
                Referencia = "REF-001"
            }
        ];

        string? caminhoTemp = Path.GetTempFileName();

        // Act
        gerador.GerarFicheiro(pagamentos, caminhoTemp);

        // Assert
        string[] linhas = File.ReadAllLines(caminhoTemp);
        Assert.HasCount(2, linhas);
        Assert.Contains("Data;Beneficiario", linhas[0]);
        Assert.Contains("Fornecedor A", linhas[1]);

        File.Delete(caminhoTemp);
    }
}