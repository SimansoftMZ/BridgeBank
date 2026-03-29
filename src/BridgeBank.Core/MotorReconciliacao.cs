using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core;

/// <summary>
/// Motor de reconciliação bancária
/// </summary>
public class MotorReconciliacao : IMotorReconciliacao
{
    private readonly List<IEstrategiaCorrespondencia> _estrategias = [];

    public MotorReconciliacao()
    {
    }

    public void RegistrarEstrategia(IEstrategiaCorrespondencia estrategia)
    {
        _estrategias.Add(estrategia);
    }

    public List<ResultadoReconciliacao> Reconciliar(
        IEnumerable<Transacao> transacoes,
        IEnumerable<LancamentoERP> lancamentos)
    {
        List<LancamentoERP> lancamentosDisponiveis = [.. lancamentos];
        List<ResultadoReconciliacao> resultados = [];

        List<IEstrategiaCorrespondencia> estrategiasOrdenadas = [.. _estrategias.OrderByDescending(e => e.Prioridade)];

        foreach (Transacao transacao in transacoes)
        {
            ResultadoReconciliacao resultado = new()
            {
                Transacao = transacao,
                TipoCorrespondencia = TipoCorrespondencia.Nenhuma,
                NivelConfianca = 0
            };

            foreach (IEstrategiaCorrespondencia estrategia in estrategiasOrdenadas)
            {
                ResultadoCorrespondencia? correspondencia = estrategia.TentarCorrespondencia(
                    transacao,
                    lancamentosDisponiveis);

                if (correspondencia != null)
                {
                    resultado.LancamentoCorrespondente = correspondencia.Lancamento;
                    resultado.TipoCorrespondencia = correspondencia.Tipo;
                    resultado.NivelConfianca = correspondencia.NivelConfianca;
                    resultado.Observacoes = correspondencia.Observacoes;

                    lancamentosDisponiveis.Remove(correspondencia.Lancamento);
                    break;
                }
            }

            resultados.Add(resultado);
        }

        return resultados;
    }
}