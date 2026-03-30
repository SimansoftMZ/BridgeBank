using Simansoft.BridgeBank.Api.Dtos;
using Simansoft.BridgeBank.Core.Models;
using Simansoft.BridgeBank.Generators.Models;

namespace Simansoft.BridgeBank.Api.Mappers;

public static class DtoMapper
{
    public static Transacao ToModel(this TransacaoDto dto) => new()
    {
        Id = dto.Id,
        Data = dto.Data,
        Valor = dto.Valor,
        Descricao = dto.Descricao,
        Referencia = dto.Referencia,
        DocumentoOrigem = dto.DocumentoOrigem,
        Tipo = Enum.Parse<TipoTransacao>(dto.Tipo, ignoreCase: true),
        Beneficiario = dto.Beneficiario,
        ContaBancaria = dto.ContaBancaria,
        Categoria = Enum.Parse<CategoriaTransacao>(dto.Categoria, ignoreCase: true),
        ConfiancaClassificacao = dto.ConfiancaClassificacao
    };

    public static TransacaoDto ToDto(this Transacao model) => new(
        model.Id,
        model.Data,
        model.Valor,
        model.Descricao,
        model.Referencia,
        model.DocumentoOrigem,
        model.Tipo.ToString(),
        model.Beneficiario,
        model.ContaBancaria,
        model.Categoria.ToString(),
        model.ConfiancaClassificacao);

    public static LancamentoERP ToModel(this LancamentoERPDto dto) => new()
    {
        Id = dto.Id,
        Data = dto.Data,
        Valor = dto.Valor,
        Descricao = dto.Descricao,
        Referencia = dto.Referencia,
        NumeroDocumento = dto.NumeroDocumento,
        Fornecedor = dto.Fornecedor,
        Cliente = dto.Cliente,
        Status = Enum.Parse<StatusLancamento>(dto.Status, ignoreCase: true)
    };

    public static LancamentoERPDto ToDto(this LancamentoERP model) => new(
        model.Id,
        model.Data,
        model.Valor,
        model.Descricao,
        model.Referencia,
        model.NumeroDocumento,
        model.Fornecedor,
        model.Cliente,
        model.Status.ToString());

    public static ResultadoClassificacaoDto ToDto(this ResultadoClassificacao model) => new(
        model.Categoria.ToString(),
        model.Confianca,
        model.RegraAplicada);

    public static ResultadoReconciliacaoDto ToDto(this ResultadoReconciliacao model) => new(
        model.Transacao.ToDto(),
        model.LancamentoCorrespondente?.ToDto(),
        model.TipoCorrespondencia.ToString(),
        model.NivelConfianca,
        model.Observacoes);

    public static Pagamento ToModel(this PagamentoDto dto) => new()
    {
        Id = dto.Id,
        DataPagamento = dto.DataPagamento,
        Valor = dto.Valor,
        Beneficiario = dto.Beneficiario,
        ContaBeneficiario = dto.ContaBeneficiario,
        BancoBeneficiario = dto.BancoBeneficiario,
        Referencia = dto.Referencia,
        Descricao = dto.Descricao,
        Tipo = Enum.Parse<TipoPagamento>(dto.Tipo, ignoreCase: true)
    };

    public static ExtratoBancarioDto ToDto(this ExtratoBancario model) => new(
        model.Banco,
        model.NumeroConta,
        model.DataInicio,
        model.DataFim,
        model.SaldoInicial,
        model.SaldoFinal,
        model.Transacoes.Select(t => t.ToDto()).ToList());
}
