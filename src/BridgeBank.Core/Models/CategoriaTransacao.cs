namespace Simansoft.BridgeBank.Core.Models;

/// <summary>
/// Categoria de classificação de uma transação bancária
/// </summary>
public enum CategoriaTransacao
{
    /// <summary>Transação não classificada</summary>
    NaoClassificada,

    /// <summary>Recebimento de cliente (pagamento de facturas, adiantamentos)</summary>
    PagamentoCliente,

    /// <summary>Pagamento ao Estado (impostos, taxas governamentais, INSS, IVA, IRPS, IRPC)</summary>
    PagamentoEstado,

    /// <summary>Pagamento a fornecedor</summary>
    PagamentoFornecedor,

    /// <summary>Despesas e encargos bancários (comissões, taxas, juros, manutenção)</summary>
    DespesaBancaria,

    /// <summary>Pagamento de salários e vencimentos</summary>
    Salario,

    /// <summary>Transferência entre contas próprias</summary>
    TransferenciaInterna,

    /// <summary>Empréstimo ou financiamento bancário</summary>
    Emprestimo,

    /// <summary>Pagamento de serviços (água, luz, telecomunicações)</summary>
    PagamentoServicos
}
