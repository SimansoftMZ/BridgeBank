namespace Simansoft.BridgeBank.Generators.Models;

/// <summary>
/// Representa um pagamento a ser processado
/// </summary>
public class Pagamento
{
    public string Id { get; set; } = string.Empty;
    public DateTime DataPagamento { get; set; }
    public decimal Valor { get; set; }
    public string Beneficiario { get; set; } = string.Empty;
    public string ContaBeneficiario { get; set; } = string.Empty;
    public string BancoBeneficiario { get; set; } = string.Empty;
    public string? Referencia { get; set; }
    public string? Descricao { get; set; }
    public TipoPagamento Tipo { get; set; }
}

public enum TipoPagamento
{
    Transferencia,
    OrdemPagamento,
    Cheque
}