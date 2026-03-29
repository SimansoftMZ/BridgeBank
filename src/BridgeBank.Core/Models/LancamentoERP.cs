namespace Simansoft.BridgeBank.Core.Models;

/// <summary>
/// Representa um lançamento do sistema ERP
/// </summary>
public class LancamentoERP
{
    public string Id { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public decimal Valor { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? Referencia { get; set; }
    public string? NumeroDocumento { get; set; }
    public string? Fornecedor { get; set; }
    public string? Cliente { get; set; }
    public StatusLancamento Status { get; set; }
}

public enum StatusLancamento
{
    Pendente,
    Reconciliado,
    Descartado
}