using Microsoft.ML;
using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.ML.Reconciliacao;

/// <summary>
/// Pipeline de treino do modelo de reconciliação (correspondência transação-ERP).
/// Usa FastTree para classificação binária sobre features numéricas.
/// </summary>
public class TreinadorReconciliacao
{
    private readonly MLContext _mlContext;

    public TreinadorReconciliacao(int? seed = 42)
    {
        _mlContext = new MLContext(seed);
    }

    /// <summary>
    /// Treina o modelo a partir de pares positivos e negativos
    /// </summary>
    public MetricasReconciliacao Treinar(
        IEnumerable<EntradaCorrespondencia> dados,
        string caminhoModelo)
    {
        IDataView dataView = _mlContext.Data.LoadFromEnumerable(dados);
        var split = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

        var pipeline = _mlContext.Transforms.Concatenate("Features",
                nameof(EntradaCorrespondencia.DiferencaValorAbsoluta),
                nameof(EntradaCorrespondencia.DiferencaValorRelativa),
                nameof(EntradaCorrespondencia.DiferencaDias),
                nameof(EntradaCorrespondencia.SimilaridadeDescricao),
                nameof(EntradaCorrespondencia.ReferenciaExata),
                nameof(EntradaCorrespondencia.ReferenciaContemSubstring),
                nameof(EntradaCorrespondencia.TipoTransacaoCoerente),
                nameof(EntradaCorrespondencia.DescricaoContemEntidade))
            .Append(_mlContext.BinaryClassification.Trainers.FastTree(
                labelColumnName: "Label",
                featureColumnName: "Features",
                numberOfLeaves: 20,
                numberOfTrees: 100,
                minimumExampleCountPerLeaf: 10,
                learningRate: 0.2));

        ITransformer modelo = pipeline.Fit(split.TrainSet);

        IDataView previsoes = modelo.Transform(split.TestSet);
        var metricas = _mlContext.BinaryClassification.Evaluate(previsoes,
            labelColumnName: "Label");

        string? directorio = Path.GetDirectoryName(caminhoModelo);
        if (!string.IsNullOrEmpty(directorio))
            Directory.CreateDirectory(directorio);

        _mlContext.Model.Save(modelo, dataView.Schema, caminhoModelo);

        return new MetricasReconciliacao
        {
            Accuracy = metricas.Accuracy,
            F1Score = metricas.F1Score,
            AreaUnderRocCurve = metricas.AreaUnderRocCurve,
            CaminhoModelo = caminhoModelo
        };
    }

    /// <summary>
    /// Gera pares de treino a partir de transações e lançamentos ERP,
    /// usando as estratégias existentes para encontrar pares positivos.
    /// </summary>
    public static List<EntradaCorrespondencia> GerarParesDeTreino(
        IEnumerable<Transacao> transacoes,
        IEnumerable<LancamentoERP> lancamentos,
        IEnumerable<ResultadoReconciliacao> resultadosConhecidos)
    {
        List<EntradaCorrespondencia> pares = [];
        var listaLancamentos = lancamentos.ToList();

        // Pares positivos: de resultados já reconciliados
        foreach (var resultado in resultadosConhecidos.Where(r => r.LancamentoCorrespondente != null))
        {
            pares.Add(ExtratorFeatures.ExtrairFeatures(resultado.Transacao, resultado.LancamentoCorrespondente!, true));
        }

        // Pares negativos: combinações que não são match
        var random = new Random(42);
        foreach (var resultado in resultadosConhecidos)
        {
            var negativos = listaLancamentos
                .Where(l => l.Id != resultado.LancamentoCorrespondente?.Id)
                .OrderBy(_ => random.Next())
                .Take(3);

            foreach (var negativo in negativos)
            {
                pares.Add(ExtratorFeatures.ExtrairFeatures(resultado.Transacao, negativo, false));
            }
        }

        return pares;
    }
}

/// <summary>
/// Métricas de avaliação do modelo de reconciliação
/// </summary>
public class MetricasReconciliacao
{
    public double Accuracy { get; set; }
    public double F1Score { get; set; }
    public double AreaUnderRocCurve { get; set; }
    public string CaminhoModelo { get; set; } = string.Empty;

    public override string ToString() =>
        $"Accuracy: {Accuracy:P2} | F1: {F1Score:P2} | AUC: {AreaUnderRocCurve:P2}";
}
