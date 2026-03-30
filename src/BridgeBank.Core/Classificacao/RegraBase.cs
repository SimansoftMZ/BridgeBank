using System.Text.RegularExpressions;

namespace Simansoft.BridgeBank.Core.Classificacao;

/// <summary>
/// Classe base para regras de classificação baseadas em palavras-chave na descrição
/// </summary>
public abstract partial class RegraClassificacaoBase
{
    /// <summary>
    /// Verifica se a descrição contém alguma das palavras-chave (case-insensitive),
    /// usando limites de palavra para evitar falsos positivos (ex: "iva" dentro de "corporativa")
    /// </summary>
    protected static bool ContemPalavraChave(string descricao, string[] palavrasChave)
    {
        return palavrasChave.Any(p => CorrespondePalavra(descricao, p));
    }

    /// <summary>
    /// Conta quantas palavras-chave aparecem na descrição
    /// </summary>
    protected static int ContarCorrespondencias(string descricao, string[] palavrasChave)
    {
        return palavrasChave.Count(p => CorrespondePalavra(descricao, p));
    }

    /// <summary>
    /// Calcula confiança baseada no número de palavras-chave encontradas
    /// </summary>
    protected static double CalcularConfianca(int correspondencias, int totalPalavras, double base_ = 0.7)
    {
        double bonus = (double)correspondencias / totalPalavras * 0.3;
        return Math.Min(base_ + bonus, 0.99);
    }

    /// <summary>
    /// Verifica se a palavra-chave existe na descrição respeitando limites de palavra.
    /// Para palavras-chave com múltiplas palavras (ex: "imposto de selo"), faz correspondência de frase.
    /// Para palavras curtas (ex: "iva"), usa word boundary para evitar matches parciais.
    /// </summary>
    private static bool CorrespondePalavra(string descricao, string palavraChave)
    {
        string escaped = Regex.Escape(palavraChave);

        // Só aplicar word boundary no início se a keyword começa com letra/dígito
        string prefixo = char.IsLetterOrDigit(palavraChave[0]) ? @"(?<!\w)" : "";

        // Só aplicar word boundary no fim se a keyword termina com letra/dígito
        string sufixo = char.IsLetterOrDigit(palavraChave[^1]) ? @"(?!\w)" : "";

        string pattern = prefixo + escaped + sufixo;
        return Regex.IsMatch(descricao, pattern, RegexOptions.IgnoreCase);
    }
}
