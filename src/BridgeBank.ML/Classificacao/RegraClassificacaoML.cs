using Microsoft.ML;
using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;
using Simansoft.BridgeBank.ML.Infraestrutura;

namespace Simansoft.BridgeBank.ML.Classificacao;

/// <summary>
/// Regra de classificação baseada em modelo ML.NET.
/// Implementa IRegraClassificacao para integração directa com ClassificadorTransacao.
/// </summary>
public class RegraClassificacaoML : IRegraClassificacao, IDisposable
{
    private readonly PredictionEngine<EntradaClassificacao, PrevisaoClassificacao> _motor;
    private readonly double _limiarConfianca;
    private bool _disposed;

    public int Prioridade { get; }
    public string Nome => "ML:ClassificacaoTexto";

    /// <summary>
    /// Cria a regra ML a partir de um ficheiro de modelo treinado
    /// </summary>
    public RegraClassificacaoML(string caminhoModelo, int prioridade = 50, double limiarConfianca = 0.5)
    {
        Prioridade = prioridade;
        _limiarConfianca = limiarConfianca;

        var mlContext = new MLContext();
        ITransformer modelo = mlContext.Model.Load(caminhoModelo, out _);
        _motor = mlContext.Model.CreatePredictionEngine<EntradaClassificacao, PrevisaoClassificacao>(modelo);
    }

    /// <summary>
    /// Cria a regra ML a partir de configuração
    /// </summary>
    public RegraClassificacaoML(ConfiguracaoML config)
        : this(config.CaminhoModeloClassificacao, config.PrioridadeClassificacao, config.LimiarConfiancaClassificacao)
    {
    }

    public ResultadoClassificacao? Classificar(Transacao transacao)
    {
        if (string.IsNullOrWhiteSpace(transacao.Descricao))
            return null;

        var entrada = new EntradaClassificacao
        {
            Descricao = transacao.Descricao,
            Valor = (float)transacao.Valor,
            TipoNumerico = transacao.Tipo == TipoTransacao.Debito ? 0f : 1f
        };

        var previsao = _motor.Predict(entrada);

        if (!Enum.TryParse<CategoriaTransacao>(previsao.CategoriaPrevisao, out var categoria))
            return null;

        if (categoria == CategoriaTransacao.NaoClassificada)
            return null;

        double confianca = previsao.Pontuacoes.Length > 0 ? previsao.Pontuacoes.Max() : 0;

        if (confianca < _limiarConfianca)
            return null;

        return new ResultadoClassificacao
        {
            Categoria = categoria,
            Confianca = confianca,
            RegraAplicada = Nome
        };
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _motor.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
