using BridgeBank.Core;
using BridgeBank.Core.Strategies;
using Simansoft.BridgeBank.Core.Models;

namespace BridgeBank.Core.Tests;

public class MotorReconciliacaoTests
{
    [Fact]
    public void Reconciliar_ComEstrategiaReferencia_DeveReconciliarCorretamente()
    {
        // Arrange
        var motor = new MotorReconciliacao();
        motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorReferencia());

        var transacoes = new List<Transacao>
        {
            new Transacao
            {
                Id = "T1",
                Data = new DateTime(2026, 1, 1),
                Valor = 1000,
                Referencia = "INV-001"
            }
        };

        var lancamentos = new List<LancamentoERP>
        {
            new LancamentoERP
            {
                Id = "L1",
                Data = new DateTime(2026, 1, 1),
                Valor = 1000,
                Referencia = "INV-001"
            }
        };

        // Act
        var resultados = motor.Reconciliar(transacoes, lancamentos);

        // Assert
        Assert.Single(resultados);
        Assert.Equal("T1", resultados[0].Transacao.Id);
        Assert.NotNull(resultados[0].LancamentoCorrespondente);
        Assert.Equal("L1", resultados[0].LancamentoCorrespondente!.Id);
        Assert.Equal(TipoCorrespondencia.PorReferencia, resultados[0].TipoCorrespondencia);
    }

    [Fact]
    public void Reconciliar_SemCorrespondencia_DeveTerTipoNenhuma()
    {
        // Arrange
        var motor = new MotorReconciliacao();
        motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorReferencia());

        var transacoes = new List<Transacao>
        {
            new Transacao
            {
                Id = "T1",
                Data = new DateTime(2026, 1, 1),
                Valor = 1000,
                Referencia = "INV-001"
            }
        };

        var lancamentos = new List<LancamentoERP>
        {
            new LancamentoERP
            {
                Id = "L1",
                Data = new DateTime(2026, 1, 1),
                Valor = 1000,
                Referencia = "INV-002"
            }
        };

        // Act
        var resultados = motor.Reconciliar(transacoes, lancamentos);

        // Assert
        Assert.Single(resultados);
        Assert.Null(resultados[0].LancamentoCorrespondente);
        Assert.Equal(TipoCorrespondencia.Nenhuma, resultados[0].TipoCorrespondencia);
    }

    [Fact]
    public void Reconciliar_ComMultiplasEstrategias_DeveUsarPrioridade()
    {
        // Arrange
        var motor = new MotorReconciliacao();
        motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorValorEData());
        motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorReferencia());

        var transacoes = new List<Transacao>
        {
            new Transacao
            {
                Id = "T1",
                Data = new DateTime(2026, 1, 1),
                Valor = 1000,
                Referencia = "INV-001"
            }
        };

        var lancamentos = new List<LancamentoERP>
        {
            new LancamentoERP
            {
                Id = "L1",
                Data = new DateTime(2026, 1, 1),
                Valor = 1000,
                Referencia = "INV-001"
            }
        };

        // Act
        var resultados = motor.Reconciliar(transacoes, lancamentos);

        // Assert
        Assert.Single(resultados);
        Assert.Equal(TipoCorrespondencia.PorReferencia, resultados[0].TipoCorrespondencia);
    }
}
