using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;
using Simansoft.BridgeBank.Core.Strategies;

namespace BridgeBank.Core.Tests;

[TestClass]
public class EstrategiaCorrespondenciaPorReferenciaTests
{
    [TestMethod]
    public void TentarCorrespondencia_ComReferenciaExata_DeveRetornarCorrespondencia()
    {
        // Arrange
        EstrategiaCorrespondenciaPorReferencia estrategia = new();
        Transacao transacao = new()
        {
            Id = "1",
            Data = DateTime.Now,
            Valor = 1000,
            Referencia = "INV-001"
        };

        List<LancamentoERP> lancamentos =
        [
            new() { Id = "1", Referencia = "INV-001", Valor = 1000, Data = DateTime.Now }
        ];

        // Act
        ResultadoCorrespondencia? resultado = estrategia.TentarCorrespondencia(transacao, lancamentos);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual("1", resultado.Lancamento.Id);
        Assert.AreEqual(TipoCorrespondencia.PorReferencia, resultado.Tipo);
        Assert.AreEqual(1.0, resultado.NivelConfianca);
    }

    [TestMethod]
    public void TentarCorrespondencia_SemReferencia_DeveRetornarNull()
    {
        // Arrange
        EstrategiaCorrespondenciaPorReferencia estrategia = new();
        Transacao transacao = new()
        {
            Id = "1",
            Data = DateTime.Now,
            Valor = 1000
        };

        List<LancamentoERP> lancamentos =
        [
            new() { Id = "1", Referencia = "INV-001", Valor = 1000 }
        ];

        // Act
        ResultadoCorrespondencia? resultado = estrategia.TentarCorrespondencia(transacao, lancamentos);

        // Assert
        Assert.IsNull(resultado);
    }
}