namespace Simansoft.BridgeBank.Api.Dtos;

/// <summary>
/// Pedido de classificação automática de transações
/// </summary>
/// <param name="Transacoes">Lista de transações a classificar</param>
public record ClassificarTransacoesRequest(List<TransacaoDto> Transacoes);

/// <summary>
/// Pedido de reconciliação entre transações bancárias e lançamentos ERP
/// </summary>
/// <param name="Transacoes">Transações bancárias a reconciliar</param>
/// <param name="LancamentosERP">Lançamentos do sistema ERP para correspondência</param>
public record ReconciliarRequest(
    List<TransacaoDto> Transacoes,
    List<LancamentoERPDto> LancamentosERP);

/// <summary>
/// Pedido de geração de ficheiro de pagamento
/// </summary>
/// <param name="Banco">Nome do banco (ex: BCI, MillenniumBIM, StandardBank)</param>
/// <param name="Pagamentos">Lista de pagamentos a incluir no ficheiro</param>
public record GerarPagamentoRequest(
    string Banco,
    List<PagamentoDto> Pagamentos);
