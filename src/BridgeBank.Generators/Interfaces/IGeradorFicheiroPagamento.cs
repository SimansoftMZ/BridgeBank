using Simansoft.BridgeBank.Generators.Models;

namespace Simansoft.BridgeBank.Generators.Interfaces;

/// <summary>
/// Interface para geradores de ficheiros de pagamento
/// </summary>
public interface IGeradorFicheiroPagamento
{
    /// <summary>
    /// Gera um ficheiro de pagamento
    /// </summary>
    void GerarFicheiro(IEnumerable<Pagamento> pagamentos, string caminhoArquivo);

    /// <summary>
    /// Formato do ficheiro gerado
    /// </summary>
    string FormatoFicheiro { get; }
}