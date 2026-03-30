using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Classificacao.Regras;

/// <summary>
/// Regra para identificar transferências entre contas próprias
/// </summary>
public class RegraTransferenciaInterna : RegraClassificacaoBase, IRegraClassificacao
{
    private static readonly string[] PalavrasChave =
    [
        "transferencia interna", "transferência interna",
        "transferencia entre contas", "transferência entre contas",
        "transf interna", "transf. interna",
        "movimento interno",
        "conta propria", "conta própria",
        "entre contas"
    ];

    public int Prioridade => 85;
    public string Nome => "Transferência Interna";

    public ResultadoClassificacao? Classificar(Transacao transacao)
    {
        if (string.IsNullOrWhiteSpace(transacao.Descricao))
            return null;

        int correspondencias = ContarCorrespondencias(transacao.Descricao, PalavrasChave);
        if (correspondencias == 0)
            return null;

        return new ResultadoClassificacao
        {
            Categoria = CategoriaTransacao.TransferenciaInterna,
            Confianca = CalcularConfianca(correspondencias, PalavrasChave.Length, 0.85),
            RegraAplicada = Nome
        };
    }
}
