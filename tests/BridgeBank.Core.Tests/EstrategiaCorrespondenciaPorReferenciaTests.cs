using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;
using Simansoft.BridgeBank.Core.Strategies;

namespace BridgeBank.Core.Tests;

public class EstrategiaCorrespondenciaPorReferenciaTests
{
    [Fact]
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
        Assert.NotNull(resultado);
        Assert.Equal("1", resultado.Lancamento.Id);
        Assert.Equal(TipoCorrespondencia.PorReferencia, resultado.Tipo);
        Assert.Equal(1.0, resultado.NivelConfianca);
    }

    [Fact]
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
        Assert.Null(resultado);
    }
}