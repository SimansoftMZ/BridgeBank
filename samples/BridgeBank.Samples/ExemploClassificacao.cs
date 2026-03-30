using Simansoft.BridgeBank.Core.Classificacao;
using Simansoft.BridgeBank.Core.Models;

namespace BridgeBank.Samples;

/// <summary>
/// Exemplo de classificação automática de transações bancárias
/// utilizando o extracto real carregado pelo utilizador.
/// </summary>
public static class ExemploClassificacao
{
    public static void Executar(ExtratoBancario extrato)
    {
        Console.WriteLine("Classificação Automática de Transações");
        Console.WriteLine(new string('=', 60));
        Console.WriteLine();

        Console.WriteLine($"Extracto: {extrato.Banco}");
        Console.WriteLine($"Conta:    {extrato.NumeroConta}");
        Console.WriteLine($"Período:  {extrato.DataInicio:dd/MM/yyyy} a {extrato.DataFim:dd/MM/yyyy}");
        Console.WriteLine($"Total:    {extrato.Transacoes.Count} transacções");
        Console.WriteLine();

        // Classificar todas as transações do extrato
        var classificador = ClassificadorTransacao.CriarComRegrasPadrao();
        List<ResultadoClassificacao> resultados = classificador.ClassificarExtrato(extrato);

        // Mostrar cada transação com a sua classificação
        Console.WriteLine($"{"Data",-12} {"Tipo",-4} {"Valor",14} {"Categoria",-24} {"Confiança",10}  Descrição");
        Console.WriteLine(new string('-', 110));

        for (int i = 0; i < extrato.Transacoes.Count; i++)
        {
            Transacao t = extrato.Transacoes[i];
            ResultadoClassificacao r = resultados[i];

            string tipo = t.Tipo == TipoTransacao.Debito ? "DEB" : "CRD";
            string sinal = t.Tipo == TipoTransacao.Debito ? "-" : "+";
            string categoria = FormatarCategoria(r.Categoria);
            string confianca = r.Confianca > 0 ? $"{r.Confianca:P0}" : "---";

            Console.WriteLine($"{t.Data:dd/MM/yyyy}   {tipo}  {sinal}{t.Valor,13:N2}  {categoria,-24} {confianca,10}  {t.Descricao}");
        }

        Console.WriteLine(new string('-', 110));
        Console.WriteLine();

        // Resumo por categoria
        MostrarResumo(extrato.Transacoes, resultados);
    }

    private static void MostrarResumo(List<Transacao> transacoes, List<ResultadoClassificacao> resultados)
    {
        Console.WriteLine("Resumo por Categoria");
        Console.WriteLine(new string('-', 60));

        var agrupamento = resultados
            .Select((r, i) => new { Resultado = r, Transacao = transacoes[i] })
            .GroupBy(x => x.Resultado.Categoria)
            .OrderByDescending(g => g.Count());

        foreach (var grupo in agrupamento)
        {
            string categoria = FormatarCategoria(grupo.Key);
            int quantidade = grupo.Count();

            decimal totalDebitos = grupo
                .Where(x => x.Transacao.Tipo == TipoTransacao.Debito)
                .Sum(x => x.Transacao.Valor);

            decimal totalCreditos = grupo
                .Where(x => x.Transacao.Tipo == TipoTransacao.Credito)
                .Sum(x => x.Transacao.Valor);

            double confiancaMedia = grupo.Average(x => x.Resultado.Confianca);

            Console.WriteLine($"  {categoria,-24} {quantidade,3} transacções");

            if (totalDebitos > 0)
                Console.WriteLine($"    {"Débitos:",-16} -{totalDebitos,14:N2} MZN");
            if (totalCreditos > 0)
                Console.WriteLine($"    {"Créditos:",-16} +{totalCreditos,14:N2} MZN");
            if (confiancaMedia > 0)
                Console.WriteLine($"    {"Confiança média:",-16} {confiancaMedia,14:P0}");

            Console.WriteLine();
        }

        // Estatísticas gerais
        int classificadas = resultados.Count(r => r.Categoria != CategoriaTransacao.NaoClassificada);
        int total = resultados.Count;

        Console.WriteLine(new string('-', 60));
        Console.WriteLine($"  Total de transacções:   {total}");
        Console.WriteLine($"  Classificadas:          {classificadas}");
        Console.WriteLine($"  Não classificadas:      {total - classificadas}");
        Console.WriteLine($"  Taxa de classificação:  {(classificadas * 100.0 / total):F1}%");
    }

    private static string FormatarCategoria(CategoriaTransacao categoria) => categoria switch
    {
        CategoriaTransacao.PagamentoCliente => "Pagamento Cliente",
        CategoriaTransacao.PagamentoEstado => "Pagamento Estado",
        CategoriaTransacao.PagamentoFornecedor => "Pagamento Fornecedor",
        CategoriaTransacao.DespesaBancaria => "Despesa Bancária",
        CategoriaTransacao.Salario => "Salário",
        CategoriaTransacao.TransferenciaInterna => "Transferência Interna",
        CategoriaTransacao.Emprestimo => "Empréstimo",
        CategoriaTransacao.PagamentoServicos => "Pagamento Serviços",
        CategoriaTransacao.NaoClassificada => "Não Classificada",
        _ => categoria.ToString()
    };
}
