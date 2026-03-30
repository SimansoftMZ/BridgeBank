namespace Simansoft.BridgeBank.Core.Models;

/// <summary>
/// Resultado da classificação automática de uma transação bancária
/// </summary>
public class ResultadoClassificacao
{
    /// <summary>Categoria atribuída à transação</summary>
    public CategoriaTransacao Categoria { get; set; }

    /// <summary>Nível de confiança da classificação (0.0 a 1.0)</summary>
    public double Confianca { get; set; }

    /// <summary>Regra que determinou a classificação</summary>
    public string RegraAplicada { get; set; } = string.Empty;
}
