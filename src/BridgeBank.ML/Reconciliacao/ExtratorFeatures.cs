using Simansoft.BridgeBank.Core.Models;

namespace Simansoft.BridgeBank.ML.Reconciliacao;

/// <summary>
/// Extrai features numéricas de um par (Transação, LançamentoERP)
/// para alimentar o modelo de reconciliação ML.
/// </summary>
public static class ExtratorFeatures
{
    /// <summary>
    /// Calcula todas as features para um par transação-lançamento
    /// </summary>
    public static EntradaCorrespondencia ExtrairFeatures(Transacao transacao, LancamentoERP lancamento, bool correspondencia = false)
    {
        return new EntradaCorrespondencia
        {
            DiferencaValorAbsoluta = (float)Math.Abs(transacao.Valor - lancamento.Valor),
            DiferencaValorRelativa = CalcularDiferencaRelativa(transacao.Valor, lancamento.Valor),
            DiferencaDias = (float)Math.Abs((transacao.Data - lancamento.Data).TotalDays),
            SimilaridadeDescricao = CalcularSimilaridadeJaccard(transacao.Descricao, lancamento.Descricao),
            ReferenciaExata = ReferenciasCoincidem(transacao.Referencia, lancamento.Referencia) ? 1f : 0f,
            ReferenciaContemSubstring = ReferenciaContemOutra(transacao.Referencia, lancamento.Referencia) ? 1f : 0f,
            TipoTransacaoCoerente = CalcularCoerenciaTipo(transacao, lancamento),
            DescricaoContemEntidade = DescricaoContemNome(transacao.Descricao, lancamento) ? 1f : 0f,
            Correspondencia = correspondencia
        };
    }

    private static float CalcularDiferencaRelativa(decimal valor1, decimal valor2)
    {
        decimal max = Math.Max(valor1, valor2);
        if (max == 0) return 0f;
        return (float)(Math.Abs(valor1 - valor2) / max);
    }

    private static float CalcularSimilaridadeJaccard(string texto1, string texto2)
    {
        if (string.IsNullOrWhiteSpace(texto1) || string.IsNullOrWhiteSpace(texto2))
            return 0f;

        var palavras1 = texto1.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
        var palavras2 = texto2.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();

        int intersecao = palavras1.Intersect(palavras2).Count();
        int uniao = palavras1.Union(palavras2).Count();

        return uniao > 0 ? (float)intersecao / uniao : 0f;
    }

    private static bool ReferenciasCoincidem(string? ref1, string? ref2)
    {
        if (string.IsNullOrWhiteSpace(ref1) || string.IsNullOrWhiteSpace(ref2))
            return false;
        return string.Equals(ref1.Trim(), ref2.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static bool ReferenciaContemOutra(string? ref1, string? ref2)
    {
        if (string.IsNullOrWhiteSpace(ref1) || string.IsNullOrWhiteSpace(ref2))
            return false;
        return ref1.Contains(ref2, StringComparison.OrdinalIgnoreCase)
            || ref2.Contains(ref1, StringComparison.OrdinalIgnoreCase);
    }

    private static float CalcularCoerenciaTipo(Transacao transacao, LancamentoERP lancamento)
    {
        // Débito bancário + Fornecedor no ERP = coerente
        if (transacao.Tipo == TipoTransacao.Debito && !string.IsNullOrEmpty(lancamento.Fornecedor))
            return 1f;
        // Crédito bancário + Cliente no ERP = coerente
        if (transacao.Tipo == TipoTransacao.Credito && !string.IsNullOrEmpty(lancamento.Cliente))
            return 1f;
        return 0f;
    }

    private static bool DescricaoContemNome(string descricao, LancamentoERP lancamento)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            return false;

        string descLower = descricao.ToLowerInvariant();

        if (!string.IsNullOrWhiteSpace(lancamento.Fornecedor) &&
            descLower.Contains(lancamento.Fornecedor.ToLowerInvariant()))
            return true;

        if (!string.IsNullOrWhiteSpace(lancamento.Cliente) &&
            descLower.Contains(lancamento.Cliente.ToLowerInvariant()))
            return true;

        return false;
    }
}
