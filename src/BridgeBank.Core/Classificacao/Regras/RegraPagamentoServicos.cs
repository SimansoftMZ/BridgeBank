using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Classificacao.Regras;

/// <summary>
/// Regra para identificar pagamentos de serviços (utilidades, telecomunicações, etc.).
/// Padrões reais: pagamentos a entidades como GOHOSTING, NOVATEL, EDM, FIPAG.
/// </summary>
public class RegraPagamentoServicos : RegraClassificacaoBase, IRegraClassificacao
{
    private static readonly string[] PalavrasChave =
    [
        // Electricidade
        "electricidade", "eletricidade", "edm",

        // Água
        "agua", "água", "fipag",

        // Telecomunicações
        "telecomunicacoes", "telecomunicações",
        "tmcel", "vodacom", "movitel",
        "internet", "telefone", "telefonia",

        // Hosting e serviços digitais (padrão real Millennium BIM)
        "gohosting", "netshop",
        "canal loja digital",

        // Aluguer e renda
        "aluguel", "aluguer", "renda",

        // Seguros
        "seguro", "seguros",

        // Combustível
        "combustivel", "combustível",
        "gasolina", "gasóleo", "gasoleo",

        // Utilidades genéricas
        "luz", "utilidades"
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
