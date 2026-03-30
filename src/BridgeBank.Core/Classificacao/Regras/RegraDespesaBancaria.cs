using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Classificacao.Regras;

/// <summary>
/// Regra para identificar despesas e encargos bancários.
/// Padrões reais: "Comissão", "COM TRF", "COM. ENT.", "Imposto Selo",
/// "Tx MENSAL", "Com VISA", "taxa", "juros", "manutenção", etc.
/// </summary>
public class RegraDespesaBancaria : RegraClassificacaoBase, IRegraClassificacao
{
    private static readonly string[] PalavrasChave =
    [
        // Comissões (padrão real: "Comissão", "COMISSAO", "COM TRF", "COM. ENT.", "Com VISA")
        "comissao", "comissão", "comissoes", "comissões",
        "com trf", "com. ent.",
        "com visa", "com vis elec",

        // Taxas e tarifas (padrão real: "Tx MENSAL NETSHOP", "taxa")
        "tx mensal",
        "taxa", "taxas",
        "tarifa", "tarifas",

        // Imposto de selo sobre comissões (padrão real: "Imposto Selo S/ a comissao POS")
        "imposto selo",
        "imposto de selo",

        // Juros bancários (padrão real NedBank: "Juros -I.S-Pag Capital")
        "juros",

        // Manutenção de conta
        "manutencao", "manutenção",
        "anuidade",

        // Encargos gerais
        "encargo", "encargos",
        "despesa bancaria", "despesa bancária",
        "despesas bancarias", "despesas bancárias",
        "servico bancario", "serviço bancário",
        "selo",

        // Devoluções de comissões (padrão real Standard Bank: "Devol.Comissao")
        "devol.comissao", "devol. comissao",
        "regularizacao devol", "regularização devol",

        // Transferências internacionais
        "swift", "spread", "iof"
    ];

    public int Prioridade => 95;
    public string Nome => "Despesa Bancária";

    public ResultadoClassificacao? Classificar(Transacao transacao)
    {
        if (string.IsNullOrWhiteSpace(transacao.Descricao))
            return null;

        // Despesas bancárias são débitos, excepto devoluções de comissões
        if (transacao.Tipo != TipoTransacao.Debito &&
            !ContemPalavraChave(transacao.Descricao, ["devol.comissao", "regularizacao devol", "regularização devol"]))
            return null;

        int correspondencias = ContarCorrespondencias(transacao.Descricao, PalavrasChave);
        if (correspondencias == 0)
            return null;

        return new ResultadoClassificacao
        {
            Categoria = CategoriaTransacao.DespesaBancaria,
            Confianca = CalcularConfianca(correspondencias, PalavrasChave.Length, 0.80),
            RegraAplicada = Nome
        };
    }
}
