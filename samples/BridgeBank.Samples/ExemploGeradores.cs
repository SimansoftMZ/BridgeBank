using BridgeBank.Generators.Bancos;
using BridgeBank.Generators.Models;

namespace BridgeBank.Samples;

/// <summary>
/// Exemplo de geração de ficheiros de pagamento para diferentes bancos.
/// </summary>
public static class ExemploGeradores
{
    public static void Executar()
    {
        Console.WriteLine("Geração de Ficheiros de Pagamento");
        Console.WriteLine(new string('-', 50));
        Console.WriteLine();
        Console.WriteLine("Formatos disponíveis:");
        Console.WriteLine("  1. Millennium BIM (CSV)");
        Console.WriteLine("  2. Standard Bank (XML)");
        Console.WriteLine("  3. BCI (TXT)");
        Console.WriteLine("  4. Gerar todos");
        Console.WriteLine();
        Console.Write("Escolha o formato: ");

        var opcao = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Pasta de destino (Enter = pasta temporária): ");
        var pasta = Console.ReadLine()?.Trim()?.Trim('"');
        if (string.IsNullOrEmpty(pasta))
            pasta = Path.GetTempPath();

        if (!Directory.Exists(pasta))
        {
            Console.WriteLine($"Pasta não encontrada: {pasta}");
            return;
        }

        var pagamentos = CriarPagamentosSimulados();

        Console.WriteLine();
        Console.WriteLine($"Pagamentos a processar: {pagamentos.Count}");
        Console.WriteLine($"Valor total: {pagamentos.Sum(p => p.Valor):N2} MZN");
        Console.WriteLine();

        switch (opcao)
        {
            case "1":
                GerarMillenniumBIM(pagamentos, pasta);
                break;
            case "2":
                GerarStandardBank(pagamentos, pasta);
                break;
            case "3":
                GerarBCI(pagamentos, pasta);
                break;
            case "4":
                GerarMillenniumBIM(pagamentos, pasta);
                GerarStandardBank(pagamentos, pasta);
                GerarBCI(pagamentos, pasta);
                break;
            default:
                Console.WriteLine("Opção inválida.");
                return;
        }
    }

    private static void GerarMillenniumBIM(List<Pagamento> pagamentos, string pasta)
    {
        var gerador = new GeradorPagamentoMillenniumBIM();
        var caminho = Path.Combine(pasta, "pagamentos_mbim.csv");
        gerador.GerarFicheiro(pagamentos, caminho);
        Console.WriteLine($"[CSV] {caminho}");
        MostrarConteudo(caminho);
    }

    private static void GerarStandardBank(List<Pagamento> pagamentos, string pasta)
    {
        var gerador = new GeradorPagamentoStandardBank();
        var caminho = Path.Combine(pasta, "pagamentos_stdbank.xml");
        gerador.GerarFicheiro(pagamentos, caminho);
        Console.WriteLine($"[XML] {caminho}");
        MostrarConteudo(caminho);
    }

    private static void GerarBCI(List<Pagamento> pagamentos, string pasta)
    {
        var gerador = new GeradorPagamentoBCI();
        var caminho = Path.Combine(pasta, "pagamentos_bci.txt");
        gerador.GerarFicheiro(pagamentos, caminho);
        Console.WriteLine($"[TXT] {caminho}");
        MostrarConteudo(caminho);
    }

    private static void MostrarConteudo(string caminho, int maxLinhas = 12)
    {
        var linhas = File.ReadAllLines(caminho);
        Console.WriteLine(new string('-', 60));
        foreach (var linha in linhas.Take(maxLinhas))
            Console.WriteLine(linha);
        if (linhas.Length > maxLinhas)
            Console.WriteLine($"  ... ({linhas.Length - maxLinhas} linhas omitidas)");
        Console.WriteLine(new string('-', 60));
        Console.WriteLine();
    }

    private static List<Pagamento> CriarPagamentosSimulados() =>
    [
        new()
        {
            Id = "P001",
            DataPagamento = new DateTime(2026, 1, 10),
            Valor = 15000.00m,
            Beneficiario = "Fornecedor ABC Limitada",
            ContaBeneficiario = "000123456789",
            BancoBeneficiario = "BCI",
            Referencia = "INV-2026-001",
            Descricao = "Pagamento factura materiais",
            Tipo = TipoPagamento.Transferencia
        },
        new()
        {
            Id = "P002",
            DataPagamento = new DateTime(2026, 1, 10),
            Valor = 8500.50m,
            Beneficiario = "Prestador Serviços XYZ",
            ContaBeneficiario = "000987654321",
            BancoBeneficiario = "Millennium BIM",
            Referencia = "SERV-456",
            Descricao = "Pagamento serviços consultoria",
            Tipo = TipoPagamento.Transferencia
        }
    ];
}
