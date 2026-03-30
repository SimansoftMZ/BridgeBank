using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Classificacao.Regras;

/// <summary>
/// Regra para identificar pagamentos ao Estado
/// (impostos, INSS, IVA, IRPS, IRPC, taxas governamentais).
/// Nota: "Imposto Selo" bancário é tratado pela regra de Despesa Bancária.
/// </summary>
public class RegraPagamentoEstado : RegraClassificacaoBase, IRegraClassificacao
{
    private static readonly string[] PalavrasChave =
    [
        "inss",
        "irps",
        "irpc",
        "imposto sobre rendimento",
        "imposto sobre o rendimento",
        "autoridade tributaria", "autoridade tributária",
        "receita fiscal",
        "duat",
        "ispc",
        "taxa governamental",
        "municipio", "município",
        "conselho municipal",
        "alfandega", "alfândega",
        "direitos aduaneiros",
        "contribuicao", "contribuição",
        "seguranca social", "segurança social",
        "obrigacao fiscal", "obrigação fiscal",
        "declaracao periodica", "declaração periódica",
        "retencao na fonte", "retenção na fonte"
    ];

    // IVA precisa de tratamento especial: só classificar se aparecer como palavra isolada
    // e num contexto fiscal (débito), não dentro de outras palavras
    private static readonly string[] PalavrasChaveIVA = ["iva"];

    public int Prioridade => 90;
    public string Nome => "Pagamento ao Estado";

    public ResultadoClassificacao? Classificar(Transacao transacao)
    {
        if (string.IsNullOrWhiteSpace(transacao.Descricao))
            return null;

        if (transacao.Tipo != TipoTransacao.Debito)
            return null;

        int correspondencias = ContarCorrespondencias(transacao.Descricao, PalavrasChave);
        correspondencias += ContarCorrespondencias(transacao.Descricao, PalavrasChaveIVA);

        if (correspondencias == 0)
            return null;

        return new ResultadoClassificacao
        {
            Categoria = CategoriaTransacao.PagamentoEstado,
            Confianca = CalcularConfianca(correspondencias, PalavrasChave.Length, 0.85),
            RegraAplicada = Nome
        };
    }
}
