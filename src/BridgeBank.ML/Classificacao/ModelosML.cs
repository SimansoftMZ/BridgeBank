using Microsoft.ML.Data;

namespace Simansoft.BridgeBank.ML.Classificacao;

/// <summary>
/// Entrada do pipeline de classificação ML.NET
/// </summary>
public class EntradaClassificacao
{
    [ColumnName("Descricao")]
    public string Descricao { get; set; } = string.Empty;

    [ColumnName("Valor")]
    public float Valor { get; set; }

    [ColumnName("TipoNumerico")]
    public float TipoNumerico { get; set; }

    [ColumnName("Label")]
    public string Categoria { get; set; } = string.Empty;
}

/// <summary>
/// Saída do pipeline de classificação ML.NET
/// </summary>
public class PrevisaoClassificacao
{
    [ColumnName("PredictedLabel")]
    public string CategoriaPrevisao { get; set; } = string.Empty;

    [ColumnName("Score")]
    public float[] Pontuacoes { get; set; } = [];
}
