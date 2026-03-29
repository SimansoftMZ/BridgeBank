using Simansoft.BridgeBank.Core;
using Simansoft.BridgeBank.Core.Models;
using Simansoft.BridgeBank.Core.Strategies;

namespace BridgeBank.Core.Tests;

public class MotorReconciliacaoTests
{
    [Fact]
    public void Reconciliar_ComEstrategiaReferencia_DeveReconciliarCorretamente()
    {
        // Arrange
        MotorReconciliacao motor = new();
        motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorReferencia());

        List<Transacao> transacoes =
        [
            new() {
                Id = "T1",
                Data = new DateTime(2026, 1, 1),
                Valor = 1000,
                Referencia = "INV-001"
            }
        ];

        List<LancamentoERP> lancamentos =
        [
            new() {
                Id = "L1",
                Data = new DateTime(2026, 1, 1),
                Valor = 1000,
                Referencia = "INV-001"
            }
        ];

        // Act
        List<ResultadoReconciliacao> resultados = motor.Reconciliar(transacoes, lancamentos);

        // Assert
        Assert.ContainsSingle(resultados);
        Assert.AreEqual("T1", resultados[0].Transacao.Id);
        Assert.IsNotNull(resultados[0].LancamentoCorrespondente);
        Assert.AreEqual("L1", resultados[0].LancamentoCorrespondente!.Id);
        Assert.AreEqual(TipoCorrespondencia.PorReferencia, resultados[0].TipoCorrespondencia);
    }

    [Fact]
    public void Reconciliar_SemCorrespondencia_DeveTerTipoNenhuma()
    {
        // Arrange
        MotorReconciliacao motor = new();
        motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorReferencia());

        List<Transacao> transacoes =
        [
            new() {
                Id = "T1",
                Data = new DateTime(2026, 1, 1),
                Valor = 1000,
                Referencia = "INV-001"
            }
        ];

        List<LancamentoERP> lancamentos =
        [
            new() {
                Id = "L1",
                Data = new DateTime(2026, 1, 1),
                Valor = 1000,
                Referencia = "INV-002"
            }
        ];

        // Act
        List<ResultadoReconciliacao> resultados = motor.Reconciliar(transacoes, lancamentos);

        // Assert
        Assert.ContainsSingle(resultados);
        Assert.IsNull(resultados[0].LancamentoCorrespondente);
        Assert.AreEqual(TipoCorrespondencia.Nenhuma, resultados[0].TipoCorrespondencia);
    }

    [Fact]
    public void Reconciliar_ComMultiplasEstrategias_DeveUsarPrioridade()
    {
        // Arrange
        MotorReconciliacao motor = new();
        motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorValorEData());
        motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorReferencia());

        List<Transacao> transacoes =
        [
            new() {
                Id = "T1",
                Data = new DateTime(2026, 1, 1),
                Valor = 1000,
                Referencia = "INV-001"
            }
        ];

        List<LancamentoERP> lancamentos =
        [
            new() {
                Id = "L1",
                Data = new DateTime(2026, 1, 1),
                Valor = 1000,
                Referencia = "INV-001"
            }
        ];

        // Act
        List<ResultadoReconciliacao> resultados = motor.Reconciliar(transacoes, lancamentos);

        // Assert
        Assert.ContainsSingle(resultados);
        Assert.AreEqual(TipoCorrespondencia.PorReferencia, resultados[0].TipoCorrespondencia);
    }
}