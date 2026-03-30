using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Classificacao.Regras;

/// <summary>
/// Regra para identificar despesas e encargos bancários
/// (comissões, taxas, juros, manutenção de conta, selos, anuidades)
/// </summary>
public class RegraDespesaBancaria : RegraClassificacaoBase, IRegraClassificacao
{
    private static readonly string[] PalavrasChave =
    [
        "comissao", "comissão", "comissoes", "comissões",
        "taxa", "taxas",
        "juros", "juro",
        "manutencao", "manutenção",
        "anuidade",
        "selo", "selos",
        "encargo", "encargos",
        "tarifa", "tarifas",
        "despesa bancaria", "despesas bancarias",
        "despesa bancária", "despesas bancárias",
        "imposto selo", "imposto de selo",
        "iof",
        "cobranca", "cobrança",
        "servico bancario", "serviço bancário",
        "swift",
        "spread"
    ];

    public int Prioridade => 95;
    public string Nome => "Despesa Bancária";

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
            Categoria = CategoriaTransacao.DespesaBancaria,
            Confianca = CalcularConfianca(correspondencias, PalavrasChave.Length, 0.80),
            RegraAplicada = Nome
        };
    }
}
