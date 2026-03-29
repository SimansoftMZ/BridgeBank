using BridgeBank.Parsers.Bancos;
using BridgeBank.Parsers.Interfaces;
using Simansoft.BridgeBank.Core.Models;

namespace BridgeBank.Samples;

/// <summary>
/// Exemplos de leitura de extractos bancários utilizando os parsers.
/// </summary>
public static class ExemploParsers
{
    private static readonly Dictionary<string, Func<ILeitorExtrato>> LeitoresDisponiveis = new()
    {
        ["1"] = () => new LeitorExtratoBCI(),
        ["2"] = () => new LeitorExtratoMillenniumBIM(),
        ["3"] = () => new LeitorExtratoStandardBank(),
        ["4"] = () => new LeitorExtratoNedBank(),
        ["5"] = () => new LeitorExtratoAccessBank(),
        ["6"] = () => new LeitorExtratoMPesa(),
    };

    public static void Executar()
    {
        Console.WriteLine("Leitura de Extractos Bancários");
        Console.WriteLine(new string('-', 50));
        Console.WriteLine();
        Console.WriteLine("Bancos disponíveis:");
        Console.WriteLine("  1. BCI (Banco Comercial e de Investimentos)");
        Console.WriteLine("  2. Millennium BIM");
        Console.WriteLine("  3. Standard Bank");
        Console.WriteLine("  4. NedBank");
        Console.WriteLine("  5. Access Bank");
        Console.WriteLine("  6. M-Pesa");
        Console.WriteLine();
        Console.Write("Escolha o banco: ");

        var opcao = Console.ReadLine()?.Trim() ?? "";

        if (!LeitoresDisponiveis.TryGetValue(opcao, out var criarLeitor))
        {
            Console.WriteLine("Opção inválida.");
            return;
        }

        Console.Write("Caminho do ficheiro de extracto: ");
        var caminho = Console.ReadLine()?.Trim()?.Trim('"') ?? "";

        if (!File.Exists(caminho))
        {
            Console.WriteLine($"Ficheiro não encontrado: {caminho}");
            return;
        }

        var leitor = criarLeitor();

        if (!leitor.SuportaArquivo(caminho))
        {
            Console.WriteLine("O leitor seleccionado não suporta este tipo de ficheiro.");
            return;
        }

        try
        {
            var extrato = leitor.LerExtrato(caminho);
            MostrarExtrato(extrato);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao ler o extracto: {ex.Message}");
        }
    }

    private static void MostrarExtrato(ExtratoBancario extrato)
    {
        Console.WriteLine();
        Console.WriteLine($"Banco:         {extrato.Banco}");
        Console.WriteLine($"Conta:         {extrato.NumeroConta}");
        Console.WriteLine($"Período:       {extrato.DataInicio:dd/MM/yyyy} a {extrato.DataFim:dd/MM/yyyy}");
        Console.WriteLine($"Saldo Inicial: {extrato.SaldoInicial:N2} MZN");
        Console.WriteLine($"Saldo Final:   {extrato.SaldoFinal:N2} MZN");
        Console.WriteLine($"Transacções:   {extrato.Transacoes.Count}");
        Console.WriteLine();

        var creditos = extrato.Transacoes.Where(t => t.Tipo == TipoTransacao.Credito).ToList();
        var debitos = extrato.Transacoes.Where(t => t.Tipo == TipoTransacao.Debito).ToList();

        Console.WriteLine($"  Créditos: {creditos.Count} ({creditos.Sum(t => t.Valor):N2} MZN)");
        Console.WriteLine($"  Débitos:  {debitos.Count} ({debitos.Sum(t => t.Valor):N2} MZN)");
        Console.WriteLine();

        // Mostrar primeiras transacções
        var limite = Math.Min(extrato.Transacoes.Count, 10);
        Console.WriteLine($"Primeiras {limite} transacções:");
        Console.WriteLine(new string('-', 90));
        Console.WriteLine($"{"Data",-12} {"Tipo",-8} {"Valor",14} {"Descrição",-50}");
        Console.WriteLine(new string('-', 90));

        foreach (var t in extrato.Transacoes.Take(limite))
        {
            var tipo = t.Tipo == TipoTransacao.Credito ? "CR" : "DB";
            var descricao = t.Descricao.Length > 50 ? t.Descricao[..47] + "..." : t.Descricao;
            Console.WriteLine($"{t.Data:dd/MM/yyyy}   {tipo,-8} {t.Valor,14:N2} {descricao,-50}");
        }

        if (extrato.Transacoes.Count > limite)
            Console.WriteLine($"  ... e mais {extrato.Transacoes.Count - limite} transacções");
    }
}
