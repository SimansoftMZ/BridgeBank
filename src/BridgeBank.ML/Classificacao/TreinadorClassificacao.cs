using Microsoft.ML;
using Simansoft.BridgeBank.ML.Dados;

namespace Simansoft.BridgeBank.ML.Classificacao;

/// <summary>
/// Pipeline de treino do modelo de classificação de transações.
/// Usa LbfgsMaximumEntropy com featurização de texto (char/word n-grams).
/// </summary>
public class TreinadorClassificacao
{
    private readonly MLContext _mlContext;

    public TreinadorClassificacao(int? seed = 42)
    {
        _mlContext = new MLContext(seed);
    }

    /// <summary>
    /// Treina o modelo com dados sintéticos gerados automaticamente
    /// </summary>
    public MetricasClassificacao TreinarComDadosSinteticos(string caminhoModelo, int exemplosPorCategoria = 200)
    {
        var dados = GeradorDadosTreino.GerarDadosClassificacao(exemplosPorCategoria);
        return Treinar(dados, caminhoModelo);
    }

    /// <summary>
    /// Treina o modelo com dados fornecidos e guarda no caminho especificado
    /// </summary>
    public MetricasClassificacao Treinar(IEnumerable<EntradaClassificacao> dados, string caminhoModelo)
    {
        IDataView dataView = _mlContext.Data.LoadFromEnumerable(dados);

        // Split 80/20 para treino e validação
        var split = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

        var pipeline = _mlContext.Transforms.Conversion
                .MapValueToKey("Label", "Label")
            .Append(_mlContext.Transforms.Text.FeaturizeText("DescricaoFeatures", "Descricao"))
            .Append(_mlContext.Transforms.NormalizeMinMax("ValorNorm", "Valor"))
            .Append(_mlContext.Transforms.NormalizeMinMax("TipoNorm", "TipoNumerico"))
            .Append(_mlContext.Transforms.Concatenate("Features",
                "DescricaoFeatures", "ValorNorm", "TipoNorm"))
            .Append(_mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy("Label", "Features"))
            .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        // Treinar
        ITransformer modelo = pipeline.Fit(split.TrainSet);

        // Avaliar
        IDataView previsoes = modelo.Transform(split.TestSet);
        var metricas = _mlContext.MulticlassClassification.Evaluate(previsoes, "Label");

        // Guardar modelo
        string? directorio = Path.GetDirectoryName(caminhoModelo);
        if (!string.IsNullOrEmpty(directorio))
            Directory.CreateDirectory(directorio);

        _mlContext.Model.Save(modelo, dataView.Schema, caminhoModelo);

        return new MetricasClassificacao
        {
            MicroAccuracy = metricas.MicroAccuracy,
            MacroAccuracy = metricas.MacroAccuracy,
            LogLoss = metricas.LogLoss,
            CaminhoModelo = caminhoModelo
        };
    }
}

/// <summary>
/// Métricas de avaliação do modelo de classificação
/// </summary>
public class MetricasClassificacao
{
    public double MicroAccuracy { get; set; }
    public double MacroAccuracy { get; set; }
    public double LogLoss { get; set; }
    public string CaminhoModelo { get; set; } = string.Empty;

    public override string ToString() =>
        $"MicroAccuracy: {MicroAccuracy:P2} | MacroAccuracy: {MacroAccuracy:P2} | LogLoss: {LogLoss:F4}";
}
