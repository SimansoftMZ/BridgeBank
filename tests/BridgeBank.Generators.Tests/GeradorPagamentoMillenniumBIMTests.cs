using Simansoft.BridgeBank.Generators.Bancos;
using Simansoft.BridgeBank.Generators.Models;

namespace BridgeBank.Generators.Tests;

[TestClass]
public class GeradorPagamentoMillenniumBIMTests
{
    [TestMethod]
    public void GerarFicheiro_DeveGerarCsvComCabecalho()
    {
        // Arrange
        GeradorPagamentoMillenniumBIM gerador = new();
        List<Pagamento> pagamentos =
        [
            new() {
                Id = "P1",
                DataPagamento = new DateTime(2026, 1, 1),
                Valor = 18500m,
                Beneficiario = "Fornecedor A",
                ContaBeneficiario = "123456789",
                BancoBeneficiario = "BCI",
                Referencia = "BIM0002"
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