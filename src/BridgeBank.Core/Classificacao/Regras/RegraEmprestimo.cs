using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Classificacao.Regras;

/// <summary>
/// Regra para identificar operações de empréstimo e financiamento
/// </summary>
public class RegraEmprestimo : RegraClassificacaoBase, IRegraClassificacao
{
    private static readonly string[] PalavrasChave =
    [
        "emprestimo", "empréstimo",
        "financiamento",
        "prestacao", "prestação",
        "amortizacao", "amortização",
        "credito bancario", "crédito bancário",
        "leasing",
        "mutuo", "mútuo",
        "divida", "dívida",
        "capital em divida", "capital em dívida"
    ];

    public int Prioridade => 82;
    public string Nome => "Empréstimo";

    public ResultadoClassificacao? Classificar(Transacao transacao)
    {
        if (string.IsNullOrWhiteSpace(transacao.Descricao))
            return null;

        int correspondencias = ContarCorrespondencias(transacao.Descricao, PalavrasChave);
        if (correspondencias == 0)
            return null;

        return new ResultadoClassificacao
        {
            Categoria = CategoriaTransacao.Emprestimo,
            Confianca = CalcularConfianca(correspondencias, PalavrasChave.Length, 0.80),
            RegraAplicada = Nome
        };
    }
}
