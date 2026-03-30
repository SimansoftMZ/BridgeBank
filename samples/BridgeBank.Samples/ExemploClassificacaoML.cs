using Simansoft.BridgeBank.Core.Classificacao;
using Simansoft.BridgeBank.Core.Models;
using Simansoft.BridgeBank.ML.Classificacao;
using Simansoft.BridgeBank.ML.Dados;

namespace BridgeBank.Samples;

/// <summary>
/// Exemplo de classificação automática de transações usando ML.NET,
/// com comparação lado-a-lado com as regras determinísticas do Core.
/// </summary>
public static class ExemploClassificacaoML
{
    public static void Executar(ExtratoBancario extrato)
    {
        Console.WriteLine("Classificação ML.NET vs Regras Determinísticas");
        Console.WriteLine(new string('=', 80));
        Console.WriteLine();
        Console.WriteLine($"Extracto: {extrato.Banco}");
        Console.WriteLine($"Conta:    {extrato.NumeroConta}");
        Console.WriteLine($"Período:  {extrato.DataInicio:dd/MM/yyyy} a {extrato.DataFim:dd/MM/yyyy}");
        Console.WriteLine($"Total:    {extrato.Transacoes.Count} transacções");
        Console.WriteLine();

        // ── Passo 1: Treinar o modelo ML ────────────────────────────────
        Console.WriteLine("Passo 1: Treinar modelo ML.NET com dados sintéticos...");
        string caminhoModelo = Path.Combine(Path.GetTempPath(), "bridgebank", "classificacao-sample.zip");
        var treinador = new TreinadorClassificacao();
        var metricas = treinador.TreinarComDadosSinteticos(caminhoModelo, exemplosPorCategoria: 300);

        Console.WriteLine($"  Modelo treinado: {metricas}");
        Console.WriteLine($"  Guardado em:     {caminhoModelo}");
        Console.WriteLine();

        // ── Passo 2: Classificar com regras e com ML ────────────────────
        Console.WriteLine("Passo 2: Classificar transacções...");
        Console.WriteLine();

        var classificadorRegras = ClassificadorTransacao.CriarComRegrasPadrao();
        List<ResultadoClassificacao> resultadosRegras = classificadorRegras.ClassificarExtrato(extrato);

        using var regraML = new RegraClassificacaoML(caminhoModelo, prioridade: 50, limiarConfianca: 0.3);
        List<ResultadoClassificacao> resultadosML = extrato.Transacoes
            .Select(t => regraML.Classificar(t) ?? new ResultadoClassificacao
            {
                Categoria = CategoriaTransacao.NaoClassificada,
                Confianca = 0,
                RegraAplicada = "---"
            })
            .ToList();

        // ── Passo 3: Tabela comparativa ─────────────────────────────────
        Console.WriteLine($"{"Data",-12} {"Tipo",-4} {"Valor",14}  {"Regras",-24} {"ML.NET",-24} {"Acordo",6}  Descrição");
        Console.WriteLine(new string('-', 130));

        int acordos = 0;
        int mlMelhor = 0;
        int regrasMelhor = 0;

        for (int i = 0; i < extrato.Transacoes.Count; i++)
        {
            Transacao t = extrato.Transacoes[i];
            ResultadoClassificacao rRegra = resultadosRegras[i];
            ResultadoClassificacao rML = resultadosML[i];

            string tipo = t.Tipo == TipoTransacao.Debito ? "DEB" : "CRD";
            string sinal = t.Tipo == TipoTransacao.Debito ? "-" : "+";
            string catRegra = FormatarCategoria(rRegra.Categoria);
            string catML = FormatarCategoria(rML.Categoria);

            bool mesmaCategoria = rRegra.Categoria == rML.Categoria;
            string acordo = mesmaCategoria ? "  ==" : "  <>";

            if (mesmaCategoria)
                acordos++;
            else if (rRegra.Categoria == CategoriaTransacao.NaoClassificada && rML.Categoria != CategoriaTransacao.NaoClassificada)
                mlMelhor++;
            else if (rRegra.Categoria != CategoriaTransacao.NaoClassificada && rML.Categoria == CategoriaTransacao.NaoClassificada)
                regrasMelhor++;

            string descricaoTruncada = t.Descricao.Length > 40 ? t.Descricao[..40] + "..." : t.Descricao;

            Console.WriteLine($"{t.Data:dd/MM/yyyy}   {tipo}  {sinal}{t.Valor,13:N2}  {catRegra,-24} {catML,-24} {acordo}  {descricaoTruncada}");
        }

        Console.WriteLine(new string('-', 130));
        Console.WriteLine();

        // ── Passo 4: Resumo comparativo ─────────────────────────────────
        MostrarResumoComparativo(extrato.Transacoes, resultadosRegras, resultadosML, acordos, mlMelhor, regrasMelhor);
    }

    private static void MostrarResumoComparativo(
        List<Transacao> transacoes,
        List<ResultadoClassificacao> regras,
        List<ResultadoClassificacao> ml,
        int acordos, int mlMelhor, int regrasMelhor)
    {
        int total = transacoes.Count;

        int classificadasRegras = regras.Count(r => r.Categoria != CategoriaTransacao.NaoClassificada);
        int classificadasML = ml.Count(r => r.Categoria != CategoriaTransacao.NaoClassificada);

        Console.WriteLine("Resumo Comparativo");
        Console.WriteLine(new string('-', 60));
        Console.WriteLine();

        // Tabela lado-a-lado
        Console.WriteLine($"  {"Métrica",-30} {"Regras",12} {"ML.NET",12}");
        Console.WriteLine($"  {new string('-', 54)}");
        Console.WriteLine($"  {"Classificadas",-30} {classificadasRegras,12} {classificadasML,12}");
        Console.WriteLine($"  {"Não classificadas",-30} {total - classificadasRegras,12} {total - classificadasML,12}");
        Console.WriteLine($"  {"Taxa de classificação",-30} {(classificadasRegras * 100.0 / total):F1}%{"",-7} {(classificadasML * 100.0 / total):F1}%");
        Console.WriteLine($"  {"Confiança média",-30} {regras.Where(r => r.Confianca > 0).DefaultIfEmpty().Average(r => r?.Confianca ?? 0):P0}{"",-8} {ml.Where(r => r.Confianca > 0).DefaultIfEmpty().Average(r => r?.Confianca ?? 0):P0}");
        Console.WriteLine();

        // Concordância
        Console.WriteLine($"  Concordância entre métodos:  {acordos}/{total} ({acordos * 100.0 / total:F1}%)");
        Console.WriteLine($"  ML classifica mas regras não: {mlMelhor}");
        Console.WriteLine($"  Regras classificam mas ML não: {regrasMelhor}");
        Console.WriteLine();

        // Resumo por categoria (ambos métodos)
        Console.WriteLine("  Distribuição por Categoria:");
        Console.WriteLine($"  {"Categoria",-24} {"Regras",8} {"ML.NET",8}");
        Console.WriteLine($"  {new string('-', 40)}");

        var todasCategorias = Enum.GetValues<CategoriaTransacao>();
        foreach (var cat in todasCategorias)
        {
            int countRegras = regras.Count(r => r.Categoria == cat);
            int countML = ml.Count(r => r.Categoria == cat);
            if (countRegras > 0 || countML > 0)
            {
                Console.WriteLine($"  {FormatarCategoria(cat),-24} {countRegras,8} {countML,8}");
            }
        }
        Console.WriteLine();

        // Destaques: onde ML classificou e regras não
        var mlExclusivos = transacoes
            .Select((t, i) => new { Transacao = t, Regra = regras[i], ML = ml[i] })
            .Where(x => x.Regra.Categoria == CategoriaTransacao.NaoClassificada
                     && x.ML.Categoria != CategoriaTransacao.NaoClassificada)
            .ToList();

        if (mlExclusivos.Count > 0)
        {
            Console.WriteLine($"  Transacções classificadas apenas pelo ML ({mlExclusivos.Count}):");
            foreach (var item in mlExclusivos.Take(10))
            {
                string desc = item.Transacao.Descricao.Length > 50
                    ? item.Transacao.Descricao[..50] + "..."
                    : item.Transacao.Descricao;
                Console.WriteLine($"    -> {FormatarCategoria(item.ML.Categoria),-22} ({item.ML.Confianca:P0})  {desc}");
            }
            if (mlExclusivos.Count > 10)
                Console.WriteLine($"    ... e mais {mlExclusivos.Count - 10}");
            Console.WriteLine();
        }

        // Divergências: onde classificaram diferente (ambos classificaram mas discordam)
        var divergencias = transacoes
            .Select((t, i) => new { Transacao = t, Regra = regras[i], ML = ml[i] })
            .Where(x => x.Regra.Categoria != CategoriaTransacao.NaoClassificada
                     && x.ML.Categoria != CategoriaTransacao.NaoClassificada
                     && x.Regra.Categoria != x.ML.Categoria)
            .ToList();

        if (divergencias.Count > 0)
        {
            Console.WriteLine($"  Divergências (ambos classificaram, mas discordam: {divergencias.Count}):");
            foreach (var item in divergencias.Take(10))
            {
                string desc = item.Transacao.Descricao.Length > 40
                    ? item.Transacao.Descricao[..40] + "..."
                    : item.Transacao.Descricao;
                Console.WriteLine($"    Regras: {FormatarCategoria(item.Regra.Categoria),-22} ML: {FormatarCategoria(item.ML.Categoria),-22} {desc}");
            }
            if (divergencias.Count > 10)
                Console.WriteLine($"    ... e mais {divergencias.Count - 10}");
        }
    }

    private static string FormatarCategoria(CategoriaTransacao categoria) => categoria switch
    {
        CategoriaTransacao.PagamentoCliente => "Pagamento Cliente",
        CategoriaTransacao.PagamentoEstado => "Pagamento Estado",
        CategoriaTransacao.PagamentoFornecedor => "Pagamento Fornecedor",
        CategoriaTransacao.DespesaBancaria => "Despesa Bancaria",
        CategoriaTransacao.Salario => "Salario",
        CategoriaTransacao.TransferenciaInterna => "Transf. Interna",
        CategoriaTransacao.Emprestimo => "Emprestimo",
        CategoriaTransacao.PagamentoServicos => "Pag. Servicos",
        CategoriaTransacao.NaoClassificada => "Nao Classificada",
        _ => categoria.ToString()
    };
}
