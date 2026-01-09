namespace BridgeBank.Core.Models;

/// <summary>
/// Resultado de uma reconciliação entre transação bancária e lançamento ERP
/// </summary>
public class ResultadoReconciliacao
{
    public Transacao Transacao { get; set; } = null!;
    public LancamentoERP? LancamentoCorrespondente { get; set; }
    public TipoCorrespondencia TipoCorrespondencia { get; set; }
    public double NivelConfianca { get; set; }
    public List<string> Observacoes { get; set; } = new();
}

public enum TipoCorrespondencia
{
    Nenhuma,
    PorReferencia,
    PorValorEData,
    PorDescricao,
    Manual
}
