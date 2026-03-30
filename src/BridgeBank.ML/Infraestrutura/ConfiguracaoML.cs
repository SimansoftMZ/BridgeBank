namespace Simansoft.BridgeBank.ML.Infraestrutura;

/// <summary>
/// Configuração dos modelos de ML
/// </summary>
public class ConfiguracaoML
{
    /// <summary>Caminho do ficheiro do modelo de classificação (.zip)</summary>
    public string CaminhoModeloClassificacao { get; set; } = "modelo-classificacao.zip";

    /// <summary>Caminho do ficheiro do modelo de reconciliação (.zip)</summary>
    public string CaminhoModeloReconciliacao { get; set; } = "modelo-reconciliacao.zip";

    /// <summary>Limiar mínimo de confiança para classificação (0.0 a 1.0)</summary>
    public double LimiarConfiancaClassificacao { get; set; } = 0.5;

    /// <summary>Limiar mínimo de confiança para reconciliação (0.0 a 1.0)</summary>
    public double LimiarConfiancaReconciliacao { get; set; } = 0.6;

    /// <summary>Prioridade da regra ML no ClassificadorTransacao (menor = fallback)</summary>
    public int PrioridadeClassificacao { get; set; } = 50;

    /// <summary>Prioridade da estratégia ML no MotorReconciliacao</summary>
    public int PrioridadeReconciliacao { get; set; } = 50;
}
