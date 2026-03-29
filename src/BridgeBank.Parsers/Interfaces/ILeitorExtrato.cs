using Simansoft.BridgeBank.Core.Models;

namespace BridgeBank.Parsers.Interfaces;

/// <summary>
/// Interface para leitores de extrato bancário
/// </summary>
public interface ILeitorExtrato
{
    /// <summary>
    /// Lê um extrato bancário de um arquivo
    /// </summary>
    ExtratoBancario LerExtrato(string caminhoArquivo);

    /// <summary>
    /// Verifica se o leitor suporta o arquivo especificado
    /// </summary>
    bool SuportaArquivo(string caminhoArquivo);
}
