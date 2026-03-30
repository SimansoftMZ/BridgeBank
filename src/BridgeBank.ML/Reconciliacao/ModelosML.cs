using Microsoft.ML.Data;

namespace Simansoft.BridgeBank.ML.Reconciliacao;

/// <summary>
/// Entrada do pipeline de reconciliação ML.NET.
/// Representa um par (Transação, LançamentoERP) com features calculadas.
/// </summary>
public class EntradaCorrespondencia
{
    [ColumnName("DiferencaValorAbsoluta")]
    public float DiferencaValorAbsoluta { get; set; }

    [ColumnName("DiferencaValorRelativa")]
    public float DiferencaValorRelativa { get; set; }

    [ColumnName("DiferencaDias")]
    public float DiferencaDias { get; set; }

    [ColumnName("SimilaridadeDescricao")]
    public float SimilaridadeDescricao { get; set; }

    [ColumnName("ReferenciaExata")]
    public float ReferenciaExata { get; set; }

    [ColumnName("ReferenciaContemSubstring")]
    public float ReferenciaContemSubstring { get; set; }

    [ColumnName("TipoTransacaoCoerente")]
    public float TipoTransacaoCoerente { get; set; }

    [ColumnName("DescricaoContemEntidade")]
    public float DescricaoContemEntidade { get; set; }

    [ColumnName("Label")]
    public bool Correspondencia { get; set; }
}

/// <summary>
/// Saída do pipeline de reconciliação ML.NET
/// </summary>
public class PrevisaoCorrespondencia
{
    [ColumnName("PredictedLabel")]
    public bool Correspondencia { get; set; }

    [ColumnName("Probability")]
    public float Probabilidade { get; set; }

    [ColumnName("Score")]
    public float Pontuacao { get; set; }
}
