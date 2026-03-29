using System.Text;
using Simansoft.BridgeBank.Generators.Interfaces;
using Simansoft.BridgeBank.Generators.Models;

namespace Simansoft.BridgeBank.Generators.Bancos;

/// <summary>
/// Gerador de ficheiros de pagamento para o BCI
/// Formato: arquivo texto com campos de tamanho fixo
/// </summary>
public class GeradorPagamentoBCI : IGeradorFicheiroPagamento
{
    public string FormatoFicheiro => "TXT";

    public void GerarFicheiro(IEnumerable<Pagamento> pagamentos, string caminhoArquivo)
    {
        List<string> linhas = [];

        // Linha de cabeçalho
        DateTime dataGeracao = DateTime.Now;
        linhas.Add(GerarCabecalho(dataGeracao, pagamentos.Count()));

        // Linhas de pagamento
        foreach (Pagamento pagamento in pagamentos)
        {
            linhas.Add(GerarLinhaPagamento(pagamento));
        }

        // Linha de rodapé
        linhas.Add(GerarRodape(pagamentos));

        File.WriteAllLines(caminhoArquivo, linhas, Encoding.UTF8);
    }

    private static string GerarCabecalho(DateTime dataGeracao, int totalPagamentos)
    {
        StringBuilder sb = new();
        sb.Append('0');                                              // Tipo de registro
        sb.Append(dataGeracao.ToString("yyyyMMdd").PadRight(8));    // Data de geração
        sb.Append(totalPagamentos.ToString().PadLeft(6, '0'));      // Total de pagamentos
        sb.Append("BCI".PadRight(20));                              // Banco
        sb.Append(" ".PadRight(44));                                // Preenchimento
        return sb.ToString();
    }

    private static string GerarLinhaPagamento(Pagamento pagamento)
    {
        StringBuilder sb = new();
        sb.Append('1');                                                      // Tipo de registro
        sb.Append(pagamento.DataPagamento.ToString("yyyyMMdd").PadRight(8)); // Data pagamento
        sb.Append(FormatarValor(pagamento.Valor).PadLeft(15, '0'));         // Valor
        sb.Append(pagamento.Beneficiario.PadRight(30).AsSpan(0, 30));    // Beneficiário
        sb.Append(pagamento.ContaBeneficiario.PadRight(20).AsSpan(0, 20));// Conta
        sb.Append((pagamento.Referencia ?? "").PadRight(20).AsSpan(0, 20));// Referência
        sb.Append(" ".PadRight(7));                                          // Preenchimento
        return sb.ToString();
    }

    private static string GerarRodape(IEnumerable<Pagamento> pagamentos)
    {
        decimal totalValor = pagamentos.Sum(p => p.Valor);
        StringBuilder sb = new();
        sb.Append('9');                                              // Tipo de registro
        sb.Append(pagamentos.Count().ToString().PadLeft(6, '0'));   // Total de registros
        sb.Append(FormatarValor(totalValor).PadLeft(15, '0'));      // Valor total
        sb.Append(" ".PadRight(79));                                 // Preenchimento
        return sb.ToString();
    }

    private static string FormatarValor(decimal valor)
    {
        return ((long)(valor * 100)).ToString();
    }
}