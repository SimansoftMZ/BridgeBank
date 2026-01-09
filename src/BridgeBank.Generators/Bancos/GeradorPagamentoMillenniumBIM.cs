using System.Text;
using BridgeBank.Generators.Interfaces;
using BridgeBank.Generators.Models;

namespace BridgeBank.Generators.Bancos;

/// <summary>
/// Gerador de ficheiros de pagamento para o Millennium BIM
/// Formato: CSV
/// </summary>
public class GeradorPagamentoMillenniumBIM : IGeradorFicheiroPagamento
{
    public string FormatoFicheiro => "CSV";

    public void GerarFicheiro(IEnumerable<Pagamento> pagamentos, string caminhoArquivo)
    {
        var linhas = new List<string>();

        // Cabeçalho
        linhas.Add("Data;Beneficiario;Conta;Banco;Valor;Referencia;Descricao");

        // Pagamentos
        foreach (var pagamento in pagamentos)
        {
            linhas.Add(GerarLinhaPagamento(pagamento));
        }

        File.WriteAllLines(caminhoArquivo, linhas, Encoding.UTF8);
    }

    private string GerarLinhaPagamento(Pagamento pagamento)
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

    private string EscaparCsv(string valor)
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
