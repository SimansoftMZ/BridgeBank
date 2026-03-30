using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Classificacao.Regras;

/// <summary>
/// Regra para identificar recebimentos de clientes.
/// Padrões reais: "Pag. Serv.", "ENTIDADE 14029 CREDITO", "CRED. ENT.",
/// "Deposito numerario", "Trf. Recebida", "OWNER'S DEPOSIT", cheques depositados,
/// "OPR" (operações de outros bancos), transferências recebidas.
/// </summary>
public class RegraPagamentoCliente : RegraClassificacaoBase, IRegraClassificacao
{
    private static readonly string[] PalavrasChave =
    [
        // Pagamentos de serviços recebidos (padrão real BCI: "Pag. Serv.")
        "pag. serv", "pag.serv", "pag serv",

        // Créditos de entidades (padrão real Millennium BIM)
        "credito", "crédito",
        "cred. ent", "cred.ent",

        // Depósitos (padrão real NedBank/Access Bank)
        "deposito numerario", "depósito numerário",
        "deposito", "depósito",
        "dep numerario", "dep numerário",
        "owner's deposit",

        // Transferências recebidas (padrão real Standard Bank)
        "trf. recebida", "trf.recebida", "trf recebida",
        "transferencia recebida", "transferência recebida",

        // Cheques depositados (padrão real)
        "cheque deposit", "chq. depositado", "chq depositado",

        // Operações de outros bancos (padrão real Millennium BIM: "OPR ABSA", "OPR SB")
        "opr ",

        // Recebimentos genéricos
        "recebimento", "recebimentos",
        "pagamento cliente", "pagamento de cliente",
        "adiantamento cliente",

        // Transferências de terceiros (padrão real Standard Bank)
        "3rdpty transfer"
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
