using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Strategies;

/// <summary>
/// Estratégia de correspondência por referência exata
/// </summary>
public class EstrategiaCorrespondenciaPorReferencia : IEstrategiaCorrespondencia
{
    public int Prioridade => 100;

    public ResultadoCorrespondencia? TentarCorrespondencia(
        Transacao transacao,
        IEnumerable<LancamentoERP> lancamentos)
    {
        if (string.IsNullOrWhiteSpace(transacao.Referencia))
            return null;

        LancamentoERP? lancamento = lancamentos.FirstOrDefault(l =>
            !string.IsNullOrWhiteSpace(l.Referencia) &&
            l.Referencia.Equals(transacao.Referencia, StringComparison.OrdinalIgnoreCase));

        if (lancamento == null)
            return null;

        return new ResultadoCorrespondencia
        {
            Lancamento = lancamento,
            Tipo = TipoCorrespondencia.PorReferencia,
            NivelConfianca = 1.0,
            Observacoes = ["Correspondência por referência exata"]
        };
    }
}