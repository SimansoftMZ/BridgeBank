namespace Simansoft.BridgeBank.Api.Dtos;

public record ClassificarTransacoesRequest(List<TransacaoDto> Transacoes);

public record ReconciliarRequest(
    List<TransacaoDto> Transacoes,
    List<LancamentoERPDto> LancamentosERP);

public record GerarPagamentoRequest(
    string Banco,
    List<PagamentoDto> Pagamentos);
