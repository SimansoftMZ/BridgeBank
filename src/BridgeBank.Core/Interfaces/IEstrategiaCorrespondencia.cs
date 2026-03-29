using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Interfaces;

/// <summary>
/// Interface para estratégias de correspondência de transações
/// </summary>
public interface IEstrategiaCorrespondencia
{
    /// <summary>
    /// Tenta encontrar correspondência entre uma transação bancária e lançamentos ERP
    /// </summary>
    ResultadoCorrespondencia? TentarCorrespondencia(
        Transacao transacao,
        IEnumerable<LancamentoERP> lancamentos);

    /// <summary>
    /// Prioridade da estratégia (maior = executada primeiro)
    /// </summary>
    int Prioridade { get; }
}

/// <summary>
/// Resultado de uma tentativa de correspondência
/// </summary>
public class ResultadoCorrespondencia
{
    public LancamentoERP Lancamento { get; set; } = null!;
    public TipoCorrespondencia Tipo { get; set; }
    public double NivelConfianca { get; set; }
    public List<string> Observacoes { get; set; } = [];
}
