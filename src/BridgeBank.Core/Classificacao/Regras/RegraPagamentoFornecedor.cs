using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Classificacao.Regras;

/// <summary>
/// Regra para identificar pagamentos a fornecedores
/// </summary>
public class RegraPagamentoFornecedor : RegraClassificacaoBase, IRegraClassificacao
{
    private static readonly string[] PalavrasChave =
    [
        "fornecedor", "fornecedores",
        "pagamento fornecedor", "pagamento a fornecedor",
        "factura fornecedor", "fatura fornecedor",
        "compra", "compras",
        "aquisicao", "aquisição",
        "material", "materiais",
        "mercadoria", "mercadorias",
        "prestador", "prestadores",
        "servico prestado", "serviço prestado",
        "ordem compra", "ordem de compra"
    ];

    public int Prioridade => 72;
    public string Nome => "Pagamento a Fornecedor";

    public ResultadoClassificacao? Classificar(Transacao transacao)
    {
        if (string.IsNullOrWhiteSpace(transacao.Descricao))
            return null;

        // Pagamentos a fornecedores são tipicamente débitos
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
