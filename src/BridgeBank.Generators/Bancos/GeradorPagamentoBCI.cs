using System.Text;
using BridgeBank.Generators.Interfaces;
using BridgeBank.Generators.Models;

namespace BridgeBank.Generators.Bancos;

/// <summary>
/// Gerador de ficheiros de pagamento para o BCI
/// Formato: arquivo texto com campos de tamanho fixo
/// </summary>
public class GeradorPagamentoBCI : IGeradorFicheiroPagamento
{
    public string FormatoFicheiro => "TXT";

    public void GerarFicheiro(IEnumerable<Pagamento> pagamentos, string caminhoArquivo)
    {
        var linhas = new List<string>();

        // Linha de cabeçalho
        var dataGeracao = DateTime.Now;
        linhas.Add(GerarCabecalho(dataGeracao, pagamentos.Count()));

        // Linhas de pagamento
        foreach (var pagamento in pagamentos)
        {
            linhas.Add(GerarLinhaPagamento(pagamento));
        }

        // Linha de rodapé
        linhas.Add(GerarRodape(pagamentos));

        File.WriteAllLines(caminhoArquivo, linhas, Encoding.UTF8);
    }

    private string GerarCabecalho(DateTime dataGeracao, int totalPagamentos)
    {
        var sb = new StringBuilder();
        sb.Append("0");                                              // Tipo de registro
        sb.Append(dataGeracao.ToString("yyyyMMdd").PadRight(8));    // Data de geração
        sb.Append(totalPagamentos.ToString().PadLeft(6, '0'));      // Total de pagamentos
        sb.Append("BCI".PadRight(20));                              // Banco
        sb.Append(" ".PadRight(44));                                // Preenchimento
        return sb.ToString();
    }

    private string GerarLinhaPagamento(Pagamento pagamento)
    {
        var sb = new StringBuilder();
        sb.Append("1");                                                      // Tipo de registro
        sb.Append(pagamento.DataPagamento.ToString("yyyyMMdd").PadRight(8)); // Data pagamento
        sb.Append(FormatarValor(pagamento.Valor).PadLeft(15, '0'));         // Valor
        sb.Append(pagamento.Beneficiario.PadRight(30).Substring(0, 30));    // Beneficiário
        sb.Append(pagamento.ContaBeneficiario.PadRight(20).Substring(0, 20));// Conta
        sb.Append((pagamento.Referencia ?? "").PadRight(20).Substring(0, 20));// Referência
        sb.Append(" ".PadRight(7));                                          // Preenchimento
        return sb.ToString();
    }

    private string GerarRodape(IEnumerable<Pagamento> pagamentos)
    {
        var totalValor = pagamentos.Sum(p => p.Valor);
        var sb = new StringBuilder();
        sb.Append("9");                                              // Tipo de registro
        sb.Append(pagamentos.Count().ToString().PadLeft(6, '0'));   // Total de registros
        sb.Append(FormatarValor(totalValor).PadLeft(15, '0'));      // Valor total
        sb.Append(" ".PadRight(79));                                 // Preenchimento
        return sb.ToString();
    }

    private string FormatarValor(decimal valor)
    {
        return ((long)(valor * 100)).ToString();
    }
}
