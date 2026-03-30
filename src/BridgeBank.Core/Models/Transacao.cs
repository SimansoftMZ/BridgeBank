namespace Simansoft.BridgeBank.Core.Models;

/// <summary>
/// Representa uma transação bancária
/// </summary>
public class Transacao
{
    public string Id { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public decimal Valor { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? Referencia { get; set; }
    public string? DocumentoOrigem { get; set; }
    public TipoTransacao Tipo { get; set; }
    public string? Beneficiario { get; set; }
    public string? ContaBancaria { get; set; }
    public CategoriaTransacao Categoria { get; set; } = CategoriaTransacao.NaoClassificada;
    public double ConfiancaClassificacao { get; set; }
}

public enum TipoTransacao
{
    Debito,
    Credito
}