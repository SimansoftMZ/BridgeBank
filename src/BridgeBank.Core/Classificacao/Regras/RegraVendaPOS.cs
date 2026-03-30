using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Classificacao.Regras;

/// <summary>
/// Regra para identificar vendas via terminal POS e e-commerce.
/// Padrões reais Millennium BIM: "POS 56361747", "CMI 56361747",
/// "ENTIDADE 14029 CREDITO", "CRED. ENT. 96001".
/// Estes são créditos de vendas ao consumidor final.
/// </summary>
public class RegraVendaPOS : RegraClassificacaoBase, IRegraClassificacao
{
    private static readonly string[] PalavrasChave =
    [
        // Terminais POS (padrão real Millennium BIM: "POS 56361747 00918")
        "pos ",

        // Card Merchant Info (padrão real: "CMI 56361747")
        "cmi ",

        // Crédito de entidades comerciais (padrão real: "ENTIDADE 14029 CREDITO")
        "entidade",

        // E-commerce
        "netshop crt",
        "e-commerce", "ecommerce"
    ];

    public int Prioridade => 78;
    public string Nome => "Venda POS/E-commerce";

    public ResultadoClassificacao? Classificar(Transacao transacao)
    {
        if (string.IsNullOrWhiteSpace(transacao.Descricao))
            return null;

        // Vendas POS são créditos; CMI (comissões do POS) são débitos
        // Ambos se classificam como operação comercial
        int correspondencias = ContarCorrespondencias(transacao.Descricao, PalavrasChave);
        if (correspondencias == 0)
            return null;

        return new ResultadoClassificacao
        {
            Categoria = CategoriaTransacao.PagamentoCliente,
            Confianca = CalcularConfianca(correspondencias, PalavrasChave.Length, 0.82),
            RegraAplicada = Nome
        };
    }
}
