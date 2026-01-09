using BridgeBank.Core.Interfaces;
using BridgeBank.Core.Models;

namespace BridgeBank.Core.Strategies;

/// <summary>
/// Estratégia de correspondência por valor e data
/// </summary>
public class EstrategiaCorrespondenciaPorValorEData : IEstrategiaCorrespondencia
{
    private readonly int _toleranciaDias;
    private readonly decimal _toleranciaValor;

    public int Prioridade => 90;

    public EstrategiaCorrespondenciaPorValorEData(
        int toleranciaDias = 2,
        decimal toleranciaValor = 0.01m)
    {
        _toleranciaDias = toleranciaDias;
        _toleranciaValor = toleranciaValor;
    }

    public ResultadoCorrespondencia? TentarCorrespondencia(
        Transacao transacao,
        IEnumerable<LancamentoERP> lancamentos)
    {
        var lancamentosCompativeis = lancamentos.Where(l =>
        {
            var diferencaDias = Math.Abs((l.Data - transacao.Data).Days);
            var diferencaValor = Math.Abs(l.Valor - transacao.Valor);

            return diferencaDias <= _toleranciaDias &&
                   diferencaValor <= _toleranciaValor;
        }).ToList();

        if (!lancamentosCompativeis.Any())
            return null;

        var melhorLancamento = lancamentosCompativeis.OrderBy(l =>
            Math.Abs((l.Data - transacao.Data).Days) +
            (double)Math.Abs(l.Valor - transacao.Valor)).First();

        var diferencaDias = Math.Abs((melhorLancamento.Data - transacao.Data).Days);
        var diferencaValor = Math.Abs(melhorLancamento.Valor - transacao.Valor);

        var nivelConfianca = 1.0 -
            (diferencaDias / (double)(_toleranciaDias + 1) * 0.3) -
            ((double)diferencaValor / (double)(_toleranciaValor + 0.01m) * 0.2);

        return new ResultadoCorrespondencia
        {
            Lancamento = melhorLancamento,
            Tipo = TipoCorrespondencia.PorValorEData,
            NivelConfianca = Math.Max(0.5, nivelConfianca),
            Observacoes = new List<string>
            {
                $"Correspondência por valor e data (diferença: {diferencaDias} dias, {diferencaValor:C} MZN)"
            }
        };
    }
}
