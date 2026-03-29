using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Strategies;

/// <summary>
/// Estratégia de correspondência por valor e data
/// </summary>
public class EstrategiaCorrespondenciaPorValorEData(
    int toleranciaDias = 2,
    decimal toleranciaValor = 0.01m) : IEstrategiaCorrespondencia
{
    private readonly int _toleranciaDias = toleranciaDias;
    private readonly decimal _toleranciaValor = toleranciaValor;

    public int Prioridade => 90;

    public ResultadoCorrespondencia? TentarCorrespondencia(
        Transacao transacao,
        IEnumerable<LancamentoERP> lancamentos)
    {
        List<LancamentoERP> lancamentosCompativeis = [.. lancamentos.Where(l =>
        {
            int diferencaDias = Math.Abs((l.Data - transacao.Data).Days);
            decimal diferencaValor = Math.Abs(l.Valor - transacao.Valor);

            return diferencaDias <= _toleranciaDias &&
                   diferencaValor <= _toleranciaValor;
        })];

        if (lancamentosCompativeis.Count == 0)
            return null;

        LancamentoERP melhorLancamento = lancamentosCompativeis.OrderBy(l =>
            Math.Abs((l.Data - transacao.Data).Days) +
            (double)Math.Abs(l.Valor - transacao.Valor)).First();

        int diferencaDias = Math.Abs((melhorLancamento.Data - transacao.Data).Days);
        decimal diferencaValor = Math.Abs(melhorLancamento.Valor - transacao.Valor);

        double nivelConfianca = 1.0 -
            (diferencaDias / (double)(_toleranciaDias + 1) * 0.3) -
            ((double)diferencaValor / (double)(_toleranciaValor + 0.01m) * 0.2);

        return new ResultadoCorrespondencia
        {
            Lancamento = melhorLancamento,
            Tipo = TipoCorrespondencia.PorValorEData,
            NivelConfianca = Math.Max(0.5, nivelConfianca),
            Observacoes =
            [
                $"Correspondência por valor e data (diferença: {diferencaDias} dias, {diferencaValor:C} MZN)"
            ]
        };
    }
}