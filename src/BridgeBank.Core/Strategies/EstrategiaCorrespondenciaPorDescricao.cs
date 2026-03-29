using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Strategies;

/// <summary>
/// Estratégia de correspondência por similaridade de descrição
/// </summary>
public class EstrategiaCorrespondenciaPorDescricao(double limiarSimilaridade = 0.7) : IEstrategiaCorrespondencia
{
    private readonly double _limiarSimilaridade = limiarSimilaridade;

    public int Prioridade => 80;

    public ResultadoCorrespondencia? TentarCorrespondencia(
        Transacao transacao,
        IEnumerable<LancamentoERP> lancamentos)
    {
        if (string.IsNullOrWhiteSpace(transacao.Descricao))
            return null;

        var lancamentosSimilares = lancamentos
            .Where(l => !string.IsNullOrWhiteSpace(l.Descricao))
            .Select(l => new
            {
                Lancamento = l,
                Similaridade = CalcularSimilaridade(
                    transacao.Descricao,
                    l.Descricao)
            })
            .Where(x => x.Similaridade >= _limiarSimilaridade)
            .OrderByDescending(x => x.Similaridade)
            .ToList();

        if (lancamentosSimilares.Count == 0)
            return null;

        var melhor = lancamentosSimilares.First();

        return new ResultadoCorrespondencia
        {
            Lancamento = melhor.Lancamento,
            Tipo = TipoCorrespondencia.PorDescricao,
            NivelConfianca = melhor.Similaridade * 0.9,
            Observacoes =
            [
                $"Correspondência por descrição (similaridade: {melhor.Similaridade:P0})"
            ]
        };
    }

    private static double CalcularSimilaridade(string texto1, string texto2)
    {
        if (string.IsNullOrWhiteSpace(texto1) || string.IsNullOrWhiteSpace(texto2))
            return 0;

        texto1 = texto1.ToLowerInvariant().Trim();
        texto2 = texto2.ToLowerInvariant().Trim();

        if (texto1 == texto2)
            return 1.0;

        var palavras1 = texto1.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
        var palavras2 = texto2.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();

        var intersecao = palavras1.Intersect(palavras2).Count();
        var uniao = palavras1.Union(palavras2).Count();

        return uniao > 0 ? (double)intersecao / uniao : 0;
    }
}
