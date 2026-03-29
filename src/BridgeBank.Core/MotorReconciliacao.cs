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
        var lancamentosDisponiveis = lancamentos.ToList();
        var resultados = new List<ResultadoReconciliacao>();

        var estrategiasOrdenadas = _estrategias
            .OrderByDescending(e => e.Prioridade)
            .ToList();

        foreach (var transacao in transacoes)
        {
            var resultado = new ResultadoReconciliacao
            {
                Transacao = transacao,
                TipoCorrespondencia = TipoCorrespondencia.Nenhuma,
                NivelConfianca = 0
            };

            foreach (var estrategia in estrategiasOrdenadas)
            {
                var correspondencia = estrategia.TentarCorrespondencia(
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