using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Classificacao.Regras;

/// <summary>
/// Regra para identificar recebimentos de clientes
/// (pagamentos de facturas, adiantamentos, receitas comerciais)
/// </summary>
public class RegraPagamentoCliente : RegraClassificacaoBase, IRegraClassificacao
{
    private static readonly string[] PalavrasChave =
    [
        "recebimento", "recebimentos",
        "pagamento cliente", "pagamento de cliente",
        "pagamento factura", "pagamento de factura",
        "pagamento fatura", "pagamento de fatura",
        "cobranca cliente", "cobrança cliente",
        "adiantamento cliente",
        "deposito cliente", "depósito cliente",
        "receita", "receitas",
        "venda", "vendas",
        "factura", "fatura",
        "cliente"
    ];

    public int Prioridade => 70;
    public string Nome => "Pagamento de Cliente";

    public ResultadoClassificacao? Classificar(Transacao transacao)
    {
        if (string.IsNullOrWhiteSpace(transacao.Descricao))
            return null;

        // Recebimentos de clientes são tipicamente créditos
        if (transacao.Tipo != TipoTransacao.Credito)
            return null;

        int correspondencias = ContarCorrespondencias(transacao.Descricao, PalavrasChave);
        if (correspondencias == 0)
            return null;

        return new ResultadoClassificacao
        {
            Categoria = CategoriaTransacao.PagamentoCliente,
            Confianca = CalcularConfianca(correspondencias, PalavrasChave.Length, 0.70),
            RegraAplicada = Nome
        };
    }
}
