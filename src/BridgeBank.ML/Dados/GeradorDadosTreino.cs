using Simansoft.BridgeBank.Core.Models;
using Simansoft.BridgeBank.ML.Classificacao;

namespace Simansoft.BridgeBank.ML.Dados;

/// <summary>
/// Gera dados de treino sintéticos para o modelo de classificação,
/// baseados nos padrões reais dos extractos bancários moçambicanos.
/// </summary>
public static class GeradorDadosTreino
{
    private static readonly Random Rng = new(42);

    /// <summary>
    /// Gera um conjunto completo de dados de treino para classificação
    /// </summary>
    public static List<EntradaClassificacao> GerarDadosClassificacao(int exemplosPorCategoria = 200)
    {
        List<EntradaClassificacao> dados = [];

        dados.AddRange(GerarExemplos(PadroesDespesaBancaria, nameof(CategoriaTransacao.DespesaBancaria), TipoTransacao.Debito, 50, 5000, exemplosPorCategoria));
        dados.AddRange(GerarExemplos(PadroesPagamentoEstado, nameof(CategoriaTransacao.PagamentoEstado), TipoTransacao.Debito, 5000, 500000, exemplosPorCategoria));
        dados.AddRange(GerarExemplos(PadroesSalario, nameof(CategoriaTransacao.Salario), TipoTransacao.Debito, 30000, 2000000, exemplosPorCategoria));
        dados.AddRange(GerarExemplos(PadroesPagamentoCliente, nameof(CategoriaTransacao.PagamentoCliente), TipoTransacao.Credito, 500, 1000000, exemplosPorCategoria));
        dados.AddRange(GerarExemplos(PadroesPagamentoFornecedor, nameof(CategoriaTransacao.PagamentoFornecedor), TipoTransacao.Debito, 1000, 500000, exemplosPorCategoria));
        dados.AddRange(GerarExemplos(PadroesTransferenciaInterna, nameof(CategoriaTransacao.TransferenciaInterna), TipoTransacao.Debito, 10000, 5000000, exemplosPorCategoria));
        dados.AddRange(GerarExemplos(PadroesEmprestimo, nameof(CategoriaTransacao.Emprestimo), TipoTransacao.Debito, 10000, 500000, exemplosPorCategoria));
        dados.AddRange(GerarExemplos(PadroesPagamentoServicos, nameof(CategoriaTransacao.PagamentoServicos), TipoTransacao.Debito, 2000, 50000, exemplosPorCategoria));
        dados.AddRange(GerarExemplos(PadroesNaoClassificada, nameof(CategoriaTransacao.NaoClassificada), TipoTransacao.Debito, 100, 100000, exemplosPorCategoria));

        // Shuffle
        return [.. dados.OrderBy(_ => Rng.Next())];
    }

    private static List<EntradaClassificacao> GerarExemplos(
        string[] padroes, string categoria, TipoTransacao tipoPredominante,
        float valorMin, float valorMax, int quantidade)
    {
        List<EntradaClassificacao> exemplos = [];
        for (int i = 0; i < quantidade; i++)
        {
            string padrao = padroes[Rng.Next(padroes.Length)];
            string descricao = ExpandirPadrao(padrao);
            float valor = valorMin + (float)(Rng.NextDouble() * (valorMax - valorMin));
            // 90% segue o tipo predominante, 10% o oposto (para robustez)
            TipoTransacao tipo = Rng.NextDouble() < 0.9 ? tipoPredominante
                : tipoPredominante == TipoTransacao.Debito ? TipoTransacao.Credito : TipoTransacao.Debito;

            exemplos.Add(new EntradaClassificacao
            {
                Descricao = descricao,
                Valor = valor,
                TipoNumerico = tipo == TipoTransacao.Debito ? 0f : 1f,
                Categoria = categoria
            });
        }
        return exemplos;
    }

    private static string ExpandirPadrao(string padrao)
    {
        return padrao
            .Replace("{REF}", GerarReferencia())
            .Replace("{NUM}", Rng.Next(10000, 99999).ToString())
            .Replace("{CONTA}", Rng.Next(10000000, 99999999).ToString())
            .Replace("{NOME}", NomesAleatorios[Rng.Next(NomesAleatorios.Length)])
            .Replace("{EMPRESA}", EmpresasAleatorias[Rng.Next(EmpresasAleatorias.Length)])
            .Replace("{MES}", MesesAleatorios[Rng.Next(MesesAleatorios.Length)]);
    }

    private static string GerarReferencia()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Range(0, 12).Select(_ => chars[Rng.Next(chars.Length)]).ToArray());
    }

    // ══════════════════════════════════════════════════════════════════
    // Padrões por categoria (baseados em extractos reais)
    // ══════════════════════════════════════════════════════════════════

    private static readonly string[] PadroesDespesaBancaria =
    [
        "Comissão de manutenção de conta",
        "Comissão bancária mensal",
        "COM. ENT. 96001 {NUM} {REF} #",
        "COM TRF INTER BALCAO INC ISRef. {REF}",
        "COM TRF BEX DIF TT NET INC ISRef. {REF}",
        "Imposto Selo S/ a comissao POS {REF} #",
        "Imposto Selo S/ a comissao {REF}",
        "Tx MENSAL NETSHOP",
        "Com VISA e IS Art 19.2.4",
        "Com Vis Elec e IS Art 19.2.4",
        "Taxa de transferência interbancária",
        "Taxa de transferência SWIFT",
        "Juros devedores do período",
        "Juros -I.S-Pag Capital-Mobiliz|Capital|{REF}",
        "Juros -I.S-Pag Capital-Mobiliz|Juros Brutos:{NUM}|{REF}",
        "Regularizacao Devol.Comissao Devol.Comissao",
        "ENTIDADE {NUM} COMISSAO",
        "Encargos bancários mensais",
        "Comissão de processamento",
        "Selo sobre comissão"
    ];

    private static readonly string[] PadroesPagamentoEstado =
    [
        "Pagamento INSS contribuições {MES}",
        "INSS contribuições mensais",
        "IVA - Declaração periódica {MES}",
        "IVA pagamento mensal {MES}",
        "IRPS retenção na fonte colaboradores",
        "IRPS pagamento mensal",
        "IRPC - Imposto sobre rendimento",
        "IRPC pagamento provisório",
        "Imposto sobre rendimento pessoas colectivas",
        "Autoridade Tributária - pagamento",
        "Contribuição segurança social {MES}",
        "DUAT pagamento anual",
        "Obrigação fiscal trimestral",
        "Declaração periódica IVA",
        "Retenção na fonte {MES}"
    ];

    private static readonly string[] PadroesSalario =
    [
        "Folha salarial {MES} 2026",
        "Folha salarial {MES} - {NUM} colaboradores",
        "Vencimento colaboradores {MES}",
        "Vencimentos {MES}",
        "Salário mensal {MES}",
        "Salários {MES} 2026",
        "Ordenados {MES}",
        "Remuneração colaboradores",
        "Folha de pagamento {MES}",
        "Subsídio de alimentação {MES}",
        "Subsídio de transporte {MES}",
        "13 salário - décimo terceiro",
        "Bónus salarial trimestral",
        "Férias colaboradores"
    ];

    private static readonly string[] PadroesPagamentoCliente =
    [
        "Pag. Serv. {NUM}",
        "Pag. Serv. {REF}",
        "POS {CONTA} {NUM} {REF} #",
        "CMI {CONTA} {NUM} {REF} #",
        "ENTIDADE {NUM} CREDITO",
        "CRED. ENT. 96001 {NUM} {REF} #",
        "Deposito numerario|Depositante:{NOME}|{REF}",
        "Deposito numerario|Depositante:{NOME}|{REF}",
        "OWNER'S DEPOSIT DEP NUMERARIORef. {REF}",
        "Trf. Recebida Internet Banking Internet Banking",
        "Trf. Recebida",
        "3rdPty Transfer {NUM} {NUM}",
        "Chq. Depositado {NUM}",
        "OPR ABSA {NUM} {EMPRESA}",
        "OPR SB {NUM} {EMPRESA}",
        "OPR FNB {NUM} {EMPRESA}",
        "OPR BANCO CEDRO {NUM} {EMPRESA}",
        "Recebimento factura cliente {EMPRESA}",
        "Depósito cliente - {EMPRESA}",
        "Pagamento cliente - {EMPRESA}",
        "INTERNAL CHEQUE DEPOSIT - NO HOLD CHQ BEX {CONTA} {REF}",
        "FOREIGN CHEQUE DEPOSIT CHQ OIC {CONTA} {REF}",
        "Transferencia recebida {EMPRESA}"
    ];

    private static readonly string[] PadroesPagamentoFornecedor =
    [
        "TRF A/F {EMPRESA} - {REF} - TRF P/O {EMPRESA} - {REF}",
        "TRF A/F {EMPRESA} - {REF}",
        "TRF P/O {EMPRESA} - {REF}",
        "FT {NUM} {EMPRESA}Ref. {REF}",
        "Pagamento fornecedor - {EMPRESA}",
        "Pagamento fornecedor {EMPRESA}",
        "Compra de materiais {EMPRESA}",
        "Compra de mercadorias {EMPRESA}",
        "Aquisição de materiais escritório",
        "Ordem de compra {NUM}",
        "NOVATEL, LDA {EMPRESA}Ref. {REF}",
        "Prestador de serviços {EMPRESA}"
    ];

    private static readonly string[] PadroesTransferenciaInterna =
    [
        "Transferência interna entre contas",
        "Transferência interna para conta poupança",
        "Trf intrab client dif balc",
        "TRF INTER BALCAO",
        "IZI TRANSFER P/ {CONTA}",
        "Constituicao de DP|Capital|{REF}",
        "Movimento interno entre contas",
        "Transf. interna {CONTA}"
    ];

    private static readonly string[] PadroesEmprestimo =
    [
        "Amortização empréstimo bancário",
        "Amortização empréstimo bancário - prestação {NUM}",
        "Prestação empréstimo mensal",
        "Financiamento - prestação {NUM}",
        "Amortização de capital",
        "Prestação leasing {NUM}",
        "Empréstimo bancário - parcela {MES}",
        "Capital em dívida - amortização"
    ];

    private static readonly string[] PadroesPagamentoServicos =
    [
        "EDM - Electricidade escritório sede",
        "EDM electricidade {MES}",
        "FIPAG - Água escritório sede",
        "FIPAG água {MES}",
        "Vodacom telecomunicações mensal",
        "Vodacom internet corporativa mensal",
        "Movitel telecomunicações",
        "TMCel comunicações {MES}",
        "805227 GOHOSTING REDE NOVARef. {REF}",
        "Canal Loja Digital",
        "Seguro automóvel mensal",
        "Seguro escritório {MES}",
        "Aluguer escritório {MES}",
        "Renda mensal escritório",
        "Combustível frota {MES}",
        "Internet corporativa {MES}"
    ];

    private static readonly string[] PadroesNaoClassificada =
    [
        "TRF ref {NUM} op {NUM}",
        "DEP numerário",
        "Movimento ref {NUM}",
        "OP {NUM}",
        "Transferencia",
        "Ref. {REF}",
        "{NUM} {REF}",
        "MOV {NUM}",
        "CR {NUM}",
        "DB {NUM}",
        "Operação {NUM}",
        "N/D"
    ];

    private static readonly string[] NomesAleatorios =
    [
        "DIANA NOELIA NHANTUMBO", "JOAO CARLOS MACUACUA", "ANA LURDES SITOE",
        "SARA RUI", "PEDRO FERNANDO COSSA", "MARIA JOSE TEMBE",
        "CARLOS ALBERTO MONDLANE", "FATIMA ABDUL SALIMO",
        "NELSON JOAQUIM BILA", "ROSA MARIA CHISSANO"
    ];

    private static readonly string[] EmpresasAleatorias =
    [
        "NOVATEL LDA", "CTZ LOGISTICOS Lda", "OLEOS MOCAMBIQUE LDA",
        "Cervejas de Moçambique SA", "BAYPORT FINANCIAL MOCAMBIQUE",
        "Papelaria Central", "Litoral Digital Lda",
        "EMPRESA DE TRANSP MULTI", "Vodacom Moçambique",
        "Mcel SA", "TDM", "GOHOSTING"
    ];

    private static readonly string[] MesesAleatorios =
    [
        "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
        "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
    ];
}
