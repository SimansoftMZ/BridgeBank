using Simansoft.BridgeBank.Core.Classificacao.Regras;
using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Classificacao;

/// <summary>
/// Motor de classificação automática de transações bancárias.
/// Aplica regras baseadas em padrões para categorizar cada transação.
/// </summary>
public class ClassificadorTransacao
{
    private readonly List<IRegraClassificacao> _regras = [];

    /// <summary>
    /// Cria um classificador com todas as regras padrão registadas
    /// </summary>
    public static ClassificadorTransacao CriarComRegrasPadrao()
    {
        var classificador = new ClassificadorTransacao();
        classificador.RegistrarRegra(new RegraDespesaBancaria());
        classificador.RegistrarRegra(new RegraPagamentoEstado());
        classificador.RegistrarRegra(new RegraTransferenciaInterna());
        classificador.RegistrarRegra(new RegraEmprestimo());
        classificador.RegistrarRegra(new RegraSalario());
        classificador.RegistrarRegra(new RegraPagamentoServicos());
        classificador.RegistrarRegra(new RegraPagamentoFornecedor());
        classificador.RegistrarRegra(new RegraPagamentoCliente());
        return classificador;
    }

    /// <summary>
    /// Regista uma regra de classificação
    /// </summary>
    public void RegistrarRegra(IRegraClassificacao regra)
    {
        _regras.Add(regra);
    }

    /// <summary>
    /// Classifica uma única transação, retornando o resultado da classificação
    /// </summary>
    public ResultadoClassificacao Classificar(Transacao transacao)
    {
        var regrasOrdenadas = _regras.OrderByDescending(r => r.Prioridade);

        foreach (var regra in regrasOrdenadas)
        {
            var resultado = regra.Classificar(transacao);
            if (resultado != null)
                return resultado;
        }

        return new ResultadoClassificacao
        {
            Categoria = CategoriaTransacao.NaoClassificada,
            Confianca = 0,
            RegraAplicada = string.Empty
        };
    }

    /// <summary>
    /// Classifica uma transação e aplica o resultado directamente na propriedade Categoria
    /// </summary>
    public ResultadoClassificacao ClassificarEAplicar(Transacao transacao)
    {
        var resultado = Classificar(transacao);
        transacao.Categoria = resultado.Categoria;
        transacao.ConfiancaClassificacao = resultado.Confianca;
        return resultado;
    }

    /// <summary>
    /// Classifica todas as transações de um extrato bancário
    /// </summary>
    public List<ResultadoClassificacao> ClassificarExtrato(ExtratoBancario extrato)
    {
        List<ResultadoClassificacao> resultados = [];

        foreach (var transacao in extrato.Transacoes)
        {
            resultados.Add(ClassificarEAplicar(transacao));
        }

        return resultados;
    }
}
