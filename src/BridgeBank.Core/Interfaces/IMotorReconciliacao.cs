using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Interfaces;

/// <summary>
/// Interface para o motor de reconciliação bancária
/// </summary>
public interface IMotorReconciliacao
{
    /// <summary>
    /// Reconcilia transações bancárias com lançamentos do ERP
    /// </summary>
    List<ResultadoReconciliacao> Reconciliar(
        IEnumerable<Transacao> transacoes,
        IEnumerable<LancamentoERP> lancamentos);

    /// <summary>
    /// Registra uma estratégia de correspondência
    /// </summary>
    void RegistrarEstrategia(IEstrategiaCorrespondencia estrategia);
}