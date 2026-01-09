using OfficeOpenXml;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("BridgeBank.Parsers.Tests")]

namespace BridgeBank.Parsers;

/// <summary>
/// Configuração do EPPlus
/// </summary>
public static class ConfiguracaoEPPlus
{
    static ConfiguracaoEPPlus()
    {
#pragma warning disable CS0618
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
#pragma warning restore CS0618
    }

    /// <summary>
    /// Inicializa a configuração do EPPlus
    /// </summary>
    public static void Inicializar()
    {
        // O construtor estático já faz a configuração
    }
}
