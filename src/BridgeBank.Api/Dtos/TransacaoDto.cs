namespace Simansoft.BridgeBank.Api.Dtos;

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

public record ExtratoBancarioDto(
    string Banco,
    string NumeroConta,
    DateTime DataInicio,
    DateTime DataFim,
    decimal SaldoInicial,
    decimal SaldoFinal,
    List<TransacaoDto> Transacoes);

public record ResultadoClassificacaoDto(
    string Categoria,
    double Confianca,
    string RegraAplicada);

public record ResultadoReconciliacaoDto(
    TransacaoDto Transacao,
    LancamentoERPDto? LancamentoCorrespondente,
    string TipoCorrespondencia,
    double NivelConfianca,
    List<string> Observacoes);

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
