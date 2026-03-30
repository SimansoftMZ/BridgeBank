using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Classificacao.Regras;

/// <summary>
/// Regra para identificar pagamentos a fornecedores.
/// Padrões reais: "TRF A/F" (transferência a favor), "TRF P/O" (por ordem),
/// "FT" (factura), pagamentos e compras.
/// </summary>
public class RegraPagamentoFornecedor : RegraClassificacaoBase, IRegraClassificacao
{
    private static readonly string[] PalavrasChave =
    [
        // Transferências a fornecedores (padrão real Millennium BIM: "TRF A/F NOVATEL")
        "trf a/f", "trf p/o",

        // Pagamentos (padrão real: "FT 10625 Anuncios Novatel")
        "fornecedor", "fornecedores",
        "pagamento fornecedor", "pagamento a fornecedor",

        // Compras
        "compra", "compras",
        "aquisicao", "aquisição",
        "mercadoria", "mercadorias",
        "ordem compra", "ordem de compra",

        // Prestadores de serviço
        "prestador", "prestadores",
        "servico prestado", "serviço prestado"
    ];

    public int Prioridade => 72;
    public string Nome => "Pagamento a Fornecedor";

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
            Categoria = CategoriaTransacao.PagamentoFornecedor,
            Confianca = CalcularConfianca(correspondencias, PalavrasChave.Length, 0.72),
            RegraAplicada = Nome
        };
    }
}
