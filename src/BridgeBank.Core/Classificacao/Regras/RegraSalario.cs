using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Classificacao.Regras;

/// <summary>
/// Regra para identificar pagamentos de salários e vencimentos
/// </summary>
public class RegraSalario : RegraClassificacaoBase, IRegraClassificacao
{
    private static readonly string[] PalavrasChave =
    [
        "salario", "salário", "salarios", "salários",
        "vencimento", "vencimentos",
        "ordenado", "ordenados",
        "folha salarial",
        "folha de pagamento",
        "remuneracao", "remuneração",
        "subsidio", "subsídio",
        "13 salario", "13º salário", "decimo terceiro", "décimo terceiro",
        "ferias", "férias",
        "bonus salarial", "bónus salarial"
    ];

    public int Prioridade => 88;
    public string Nome => "Salário";

    public ResultadoClassificacao? Classificar(Transacao transacao)
    {
        if (string.IsNullOrWhiteSpace(transacao.Descricao))
            return null;

        if (transacao.Tipo != TipoTransacao.Debito)
            return null;

        int correspondencias = ContarCorrespondencias(transacao.Descricao, PalavrasChave);
        if (correspondencias == 0)
            return null;

        return new ResultadoClassificacao
        {
            Categoria = CategoriaTransacao.Salario,
            Confianca = CalcularConfianca(correspondencias, PalavrasChave.Length, 0.82),
            RegraAplicada = Nome
        };
    }
}
