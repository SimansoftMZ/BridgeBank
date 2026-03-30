using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Classificacao.Regras;

/// <summary>
/// Regra para identificar pagamentos de serviços (utilidades, telecomunicações, etc.)
/// </summary>
public class RegraPagamentoServicos : RegraClassificacaoBase, IRegraClassificacao
{
    private static readonly string[] PalavrasChave =
    [
        "electricidade", "eletricidade", "edm",
        "agua", "água", "fipag",
        "telecomunicacoes", "telecomunicações",
        "tmcel", "vodacom", "movitel",
        "internet",
        "telefone", "telefonia",
        "aluguel", "aluguer", "renda",
        "seguro", "seguros",
        "combustivel", "combustível",
        "gasolina", "gasóleo",
        "luz",
        "utilidades"
    ];

    public int Prioridade => 75;
    public string Nome => "Pagamento de Serviços";

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
            Categoria = CategoriaTransacao.PagamentoServicos,
            Confianca = CalcularConfianca(correspondencias, PalavrasChave.Length, 0.75),
            RegraAplicada = Nome
        };
    }
}
