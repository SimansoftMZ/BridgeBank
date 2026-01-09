using BridgeBank.Core.Models;
using BridgeBank.Core.Strategies;

namespace BridgeBank.Core.Tests;

public class EstrategiaCorrespondenciaPorReferenciaTests
{
    [Fact]
    public void TentarCorrespondencia_ComReferenciaExata_DeveRetornarCorrespondencia()
    {
        // Arrange
        var estrategia = new EstrategiaCorrespondenciaPorReferencia();
        var transacao = new Transacao
        {
            Id = "1",
            Data = DateTime.Now,
            Valor = 1000,
            Referencia = "INV-001"
        };

        var lancamentos = new List<LancamentoERP>
        {
            new LancamentoERP { Id = "1", Referencia = "INV-001", Valor = 1000, Data = DateTime.Now }
        };

        // Act
        var resultado = estrategia.TentarCorrespondencia(transacao, lancamentos);

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
        var estrategia = new EstrategiaCorrespondenciaPorReferencia();
        var transacao = new Transacao
        {
            Id = "1",
            Data = DateTime.Now,
            Valor = 1000
        };

        var lancamentos = new List<LancamentoERP>
        {
            new LancamentoERP { Id = "1", Referencia = "INV-001", Valor = 1000 }
        };

        // Act
        var resultado = estrategia.TentarCorrespondencia(transacao, lancamentos);

        // Assert
        Assert.Null(resultado);
    }
}
