using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Classificacao.Regras;

/// <summary>
/// Regra para identificar transferências entre contas próprias.
/// Padrões reais: "Trf intrab" (NedBank), "TRF INTER BALCAO" (Millennium BIM),
/// "IZI TRANSFER" (mobile), "Transferencia" (Standard Bank).
/// </summary>
public class RegraTransferenciaInterna : RegraClassificacaoBase, IRegraClassificacao
{
    private static readonly string[] PalavrasChave =
    [
        // Padrões reais
        "trf intrab", "transferencia intrab", "transferência intrab",
        "trf inter balcao", "trf inter balcão",
        "transferencia interna", "transferência interna",
        "transf interna", "transf. interna",
        "izi transfer",

        // Genéricos
        "movimento interno",
        "conta propria", "conta própria",
        "entre contas",

        // Depósitos a prazo (padrão real NedBank: "Constituicao de DP")
        "constituicao de dp", "constituição de dp"
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
