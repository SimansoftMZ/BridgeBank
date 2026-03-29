using System.Globalization;
using System.Text.RegularExpressions;

namespace Simansoft.BridgeBank.Parsers.Util;

/// <summary>
/// Utilitário para parsing de valores monetários em formatos variados
/// utilizados por bancos moçambicanos (ex: "1.234,56", "+200,00 MZN", "-318.112,16")
/// </summary>
public static partial class ParseadorNumerico
{
    private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

    /// <summary>
    /// Converte um texto representando um valor monetário em decimal.
    /// Suporta formato português (1.234,56) e internacional (1,234.56).
    /// Remove prefixos de sinal (+/-) e sufixos de moeda (MZN, etc.).
    /// </summary>
    public static decimal ParsearValorMonetario(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return 0;

        string limpo = texto.Trim();

        // Remover sufixo de moeda (ex: " MZN", " USD")
        limpo = SufixoMoedaRegex().Replace(limpo, "").Trim();

        // Remover espaços internos (ex: "3 371 027.03" ou "132 829.99")
        limpo = limpo.Replace(" ", "");

        // Remover sinal de mais (preservar o menos)
        if (limpo.StartsWith('+'))
            limpo = limpo[1..];

        if (string.IsNullOrEmpty(limpo))
            return 0;

        int lastComma = limpo.LastIndexOf(',');
        int lastDot = limpo.LastIndexOf('.');

        if (lastComma > lastDot)
        {
            // Formato português: pontos são milhares, vírgula é decimal
            // Ex: "1.234.567,89" → "1234567.89"
            limpo = limpo.Replace(".", "").Replace(",", ".");
        }
        else if (lastDot > lastComma && lastComma >= 0)
        {
            // Formato internacional: vírgulas são milhares, ponto é decimal
            // Ex: "1,234,567.89" → "1234567.89"
            limpo = limpo.Replace(",", "");
        }
        else if (lastDot >= 0 && lastComma < 0)
        {
            // Apenas pontos - verificar se o último ponto é decimal
            int dotCount = limpo.Count(c => c == '.');
            if (dotCount > 1)
            {
                // Múltiplos pontos: todos excepto o último são milhares
                // Ex: "3.371.027.03" → "3371027.03"
                int lastDotIdx = limpo.LastIndexOf('.');
                limpo = limpo[..lastDotIdx].Replace(".", "") + "." + limpo[(lastDotIdx + 1)..];
            }
            // Ponto único: tratar como decimal (ex: "1234.56")
        }

        return decimal.TryParse(limpo, NumberStyles.Any, InvariantCulture, out decimal resultado)
            ? resultado
            : 0;
    }

    [GeneratedRegex(@"\s*[A-Z]{3}\s*$")]
    private static partial Regex SufixoMoedaRegex();
}