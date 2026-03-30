using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.Core.Interfaces;

/// <summary>
/// Interface para regras de classificação automática de transações bancárias
/// </summary>
public interface IRegraClassificacao
{
    /// <summary>
    /// Tenta classificar uma transação bancária
    /// </summary>
    /// <returns>Resultado da classificação, ou null se a regra não se aplicar</returns>
    ResultadoClassificacao? Classificar(Transacao transacao);

    /// <summary>
    /// Prioridade da regra (maior = executada primeiro)
    /// </summary>
    int Prioridade { get; }

    /// <summary>
    /// Nome descritivo da regra
    /// </summary>
    string Nome { get; }
}
