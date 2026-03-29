using System.Text;
using Simansoft.BridgeBank.Generators.Interfaces;
using Simansoft.BridgeBank.Generators.Models;

namespace Simansoft.BridgeBank.Generators.Bancos;

/// <summary>
/// Gerador de ficheiros de pagamento para o Millennium BIM
/// Formato: CSV
/// </summary>
public class GeradorPagamentoMillenniumBIM : IGeradorFicheiroPagamento
{
    public string FormatoFicheiro => "CSV";

    public void GerarFicheiro(IEnumerable<Pagamento> pagamentos, string caminhoArquivo)
    {
        List<string> linhas =
        [
            // Cabeçalho
            "Data;Beneficiario;Conta;Banco;Valor;Referencia;Descricao"
        ];

        // Pagamentos
        foreach (Pagamento pagamento in pagamentos)
        {
            linhas.Add(GerarLinhaPagamento(pagamento));
        }

        File.WriteAllLines(caminhoArquivo, linhas, Encoding.UTF8);
    }

    private static string GerarLinhaPagamento(Pagamento pagamento)
    {
        return string.Join(";",
            pagamento.DataPagamento.ToString("dd/MM/yyyy"),
            EscaparCsv(pagamento.Beneficiario),
            EscaparCsv(pagamento.ContaBeneficiario),
            EscaparCsv(pagamento.BancoBeneficiario),
            pagamento.Valor.ToString("F2"),
            EscaparCsv(pagamento.Referencia ?? ""),
            EscaparCsv(pagamento.Descricao ?? "")
        );
    }

    private static string EscaparCsv(string valor)
    {
        if (string.IsNullOrEmpty(valor))
            return "";

        if (valor.Contains(';') || valor.Contains('"') || valor.Contains('\n'))
        {
            return $"\"{valor.Replace("\"", "\"\"")}\"";
        }

        return valor;
    }
}
