namespace Simansoft.BridgeBank.Api.Dtos;

/// <summary>
/// Transação bancária
/// </summary>
/// <param name="Id">Identificador único da transação</param>
/// <param name="Data">Data da transação</param>
/// <param name="Valor">Valor monetário da transação</param>
/// <param name="Descricao">Descrição ou detalhe da transação</param>
/// <param name="Referencia">Referência bancária (ex: número de factura)</param>
/// <param name="DocumentoOrigem">Documento que originou a transação</param>
/// <param name="Tipo">Tipo da transação: Debito ou Credito</param>
/// <param name="Beneficiario">Nome do beneficiário</param>
/// <param name="ContaBancaria">Número da conta bancária</param>
/// <param name="Categoria">Categoria de classificação (ex: Salario, DespesaBancaria, PagamentoFornecedor)</param>
/// <param name="ConfiancaClassificacao">Nível de confiança da classificação (0.0 a 1.0)</param>
public record TransacaoDto(
    string Id,
    DateTime Data,
    decimal Valor,
    string Descricao,
    string? Referencia,
    string? DocumentoOrigem,
    string Tipo,
    string? Beneficiario,
    string? ContaBancaria,
    string Categoria,
    double ConfiancaClassificacao);

/// <summary>
/// Lançamento do sistema ERP
/// </summary>
/// <param name="Id">Identificador único do lançamento</param>
/// <param name="Data">Data do lançamento</param>
/// <param name="Valor">Valor monetário</param>
/// <param name="Descricao">Descrição do lançamento</param>
/// <param name="Referencia">Referência do documento (ex: número de factura)</param>
/// <param name="NumeroDocumento">Número do documento contabilístico</param>
/// <param name="Fornecedor">Nome do fornecedor, se aplicável</param>
/// <param name="Cliente">Nome do cliente, se aplicável</param>
/// <param name="Status">Estado do lançamento: Pendente, Reconciliado ou Descartado</param>
public record LancamentoERPDto(
    string Id,
    DateTime Data,
    decimal Valor,
    string Descricao,
    string? Referencia,
    string? NumeroDocumento,
    string? Fornecedor,
    string? Cliente,
    string Status);

/// <summary>
/// Extrato bancário com metadados e lista de transações
/// </summary>
/// <param name="Banco">Nome do banco</param>
/// <param name="NumeroConta">Número da conta bancária</param>
/// <param name="DataInicio">Data de início do período do extrato</param>
/// <param name="DataFim">Data de fim do período do extrato</param>
/// <param name="SaldoInicial">Saldo no início do período</param>
/// <param name="SaldoFinal">Saldo no final do período</param>
/// <param name="Transacoes">Lista de transações do extrato</param>
public record ExtratoBancarioDto(
    string Banco,
    string NumeroConta,
    DateTime DataInicio,
    DateTime DataFim,
    decimal SaldoInicial,
    decimal SaldoFinal,
    List<TransacaoDto> Transacoes);

/// <summary>
/// Resultado da classificação automática de uma transação
/// </summary>
/// <param name="Categoria">Categoria atribuída (ex: Salario, DespesaBancaria)</param>
/// <param name="Confianca">Nível de confiança da classificação (0.0 a 1.0)</param>
/// <param name="RegraAplicada">Nome da regra que determinou a classificação</param>
public record ResultadoClassificacaoDto(
    string Categoria,
    double Confianca,
    string RegraAplicada);

/// <summary>
/// Resultado da reconciliação entre uma transação bancária e um lançamento ERP
/// </summary>
/// <param name="Transacao">Transação bancária</param>
/// <param name="LancamentoCorrespondente">Lançamento ERP correspondente (null se não encontrado)</param>
/// <param name="TipoCorrespondencia">Método de correspondência: Nenhuma, PorReferencia, PorValorEData, PorDescricao, PorML</param>
/// <param name="NivelConfianca">Nível de confiança da correspondência (0.0 a 1.0)</param>
/// <param name="Observacoes">Notas sobre a correspondência</param>
public record ResultadoReconciliacaoDto(
    TransacaoDto Transacao,
    LancamentoERPDto? LancamentoCorrespondente,
    string TipoCorrespondencia,
    double NivelConfianca,
    List<string> Observacoes);

/// <summary>
/// Pagamento a ser processado e incluído no ficheiro de pagamento
/// </summary>
/// <param name="Id">Identificador único do pagamento</param>
/// <param name="DataPagamento">Data do pagamento</param>
/// <param name="Valor">Valor a pagar</param>
/// <param name="Beneficiario">Nome do beneficiário</param>
/// <param name="ContaBeneficiario">Número da conta do beneficiário</param>
/// <param name="BancoBeneficiario">Banco do beneficiário</param>
/// <param name="Referencia">Referência do pagamento</param>
/// <param name="Descricao">Descrição do pagamento</param>
/// <param name="Tipo">Tipo: Transferencia, OrdemPagamento ou Cheque</param>
public record PagamentoDto(
    string Id,
    DateTime DataPagamento,
    decimal Valor,
    string Beneficiario,
    string ContaBeneficiario,
    string BancoBeneficiario,
    string? Referencia,
    string? Descricao,
    string Tipo);
