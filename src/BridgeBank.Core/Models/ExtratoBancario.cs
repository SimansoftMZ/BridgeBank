namespace Simansoft.BridgeBank.Core.Models;

/// <summary>
/// Representa um extrato bancário
/// </summary>
public class ExtratoBancario
{
    public string Banco { get; set; } = string.Empty;
    public string NumeroConta { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public decimal SaldoInicial { get; set; }
    public decimal SaldoFinal { get; set; }
    public List<Transacao> Transacoes { get; set; } = [];
}
