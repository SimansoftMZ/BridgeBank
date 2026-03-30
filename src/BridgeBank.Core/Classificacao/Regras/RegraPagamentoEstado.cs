using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Classificacao.Regras;

/// <summary>
/// Regra para identificar pagamentos ao Estado
/// (impostos, INSS, IVA, IRPS, IRPC, taxas governamentais)
/// </summary>
public class RegraPagamentoEstado : RegraClassificacaoBase, IRegraClassificacao
{
    private static readonly string[] PalavrasChave =
    [
        "inss",
        "iva",
        "irps",
        "irpc",
        "imposto", "impostos",
        "autoridade tributaria", "autoridade tributária",
        "at -", "at-",
        "receita fiscal",
        "duat",
        "ispc",
        "taxa governamental",
        "municipio", "município",
        "conselho municipal",
        "alfandega", "alfândega",
        "direitos aduaneiros",
        "contrib", "contribuicao", "contribuição",
        "seguranca social", "segurança social",
        "obrigacao fiscal", "obrigação fiscal"
    ];

    public int Prioridade => 90;
    public string Nome => "Pagamento ao Estado";

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
            Categoria = CategoriaTransacao.PagamentoEstado,
            Confianca = CalcularConfianca(correspondencias, PalavrasChave.Length, 0.85),
            RegraAplicada = Nome
        };
    }
}
