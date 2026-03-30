using Microsoft.ML;
using Simansoft.BridgeBank.Core.Interfaces;
using Simansoft.BridgeBank.Core.Models;
using Simansoft.BridgeBank.ML.Infraestrutura;

namespace Simansoft.BridgeBank.ML.Reconciliacao;

/// <summary>
/// Estratégia de correspondência baseada em modelo ML.NET.
/// Implementa IEstrategiaCorrespondencia para integração com MotorReconciliacao.
/// </summary>
public class EstrategiaCorrespondenciaML : IEstrategiaCorrespondencia, IDisposable
{
    private readonly PredictionEngine<EntradaCorrespondencia, PrevisaoCorrespondencia> _motor;
    private readonly double _limiarConfianca;
    private readonly object _lock = new();
    private bool _disposed;

    public int Prioridade { get; }

    /// <summary>
    /// Cria a estratégia ML a partir de um ficheiro de modelo treinado
    /// </summary>
    public EstrategiaCorrespondenciaML(string caminhoModelo, int prioridade = 50, double limiarConfianca = 0.6)
    {
        Prioridade = prioridade;
        _limiarConfianca = limiarConfianca;

        var mlContext = new MLContext();
        ITransformer modelo = mlContext.Model.Load(caminhoModelo, out _);
        _motor = mlContext.Model.CreatePredictionEngine<EntradaCorrespondencia, PrevisaoCorrespondencia>(modelo);
    }

    /// <summary>
    /// Cria a estratégia ML a partir de configuração
    /// </summary>
    public EstrategiaCorrespondenciaML(ConfiguracaoML config)
        : this(config.CaminhoModeloReconciliacao, config.PrioridadeReconciliacao, config.LimiarConfiancaReconciliacao)
    {
    }

    public ResultadoCorrespondencia? TentarCorrespondencia(
        Transacao transacao,
        IEnumerable<LancamentoERP> lancamentos)
    {
        LancamentoERP? melhorLancamento = null;
        float melhorProbabilidade = 0f;

        foreach (var lancamento in lancamentos)
        {
            var features = ExtratorFeatures.ExtrairFeatures(transacao, lancamento);
            PrevisaoCorrespondencia previsao;
            lock (_lock)
            {
                previsao = _motor.Predict(features);
            }

            if (previsao.Correspondencia && previsao.Probabilidade > melhorProbabilidade)
            {
                melhorProbabilidade = previsao.Probabilidade;
                melhorLancamento = lancamento;
            }
        }

        if (melhorLancamento == null || melhorProbabilidade < _limiarConfianca)
            return null;

        return new ResultadoCorrespondencia
        {
            Lancamento = melhorLancamento,
            Tipo = TipoCorrespondencia.PorML,
            NivelConfianca = melhorProbabilidade,
            Observacoes = [$"Correspondência por ML (probabilidade: {melhorProbabilidade:P0})"]
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
