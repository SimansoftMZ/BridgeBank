using System.Text;
using BridgeBank.Generators.Interfaces;
using BridgeBank.Generators.Models;

namespace BridgeBank.Generators.Bancos;

/// <summary>
/// Gerador de ficheiros de pagamento para o Standard Bank
/// Formato: XML
/// </summary>
public class GeradorPagamentoStandardBank : IGeradorFicheiroPagamento
{
    public string FormatoFicheiro => "XML";

    public void GerarFicheiro(IEnumerable<Pagamento> pagamentos, string caminhoArquivo)
    {
        var xml = new StringBuilder();
        xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        xml.AppendLine("<LotePagamentos>");
        xml.AppendLine($"  <DataGeracao>{DateTime.Now:yyyy-MM-dd HH:mm:ss}</DataGeracao>");
        xml.AppendLine($"  <TotalPagamentos>{pagamentos.Count()}</TotalPagamentos>");
        xml.AppendLine($"  <ValorTotal>{pagamentos.Sum(p => p.Valor):F2}</ValorTotal>");
        xml.AppendLine("  <Pagamentos>");

        foreach (var pagamento in pagamentos)
        {
            xml.AppendLine("    <Pagamento>");
            xml.AppendLine($"      <Id>{EscaparXml(pagamento.Id)}</Id>");
            xml.AppendLine($"      <DataPagamento>{pagamento.DataPagamento:yyyy-MM-dd}</DataPagamento>");
            xml.AppendLine($"      <Valor>{pagamento.Valor:F2}</Valor>");
            xml.AppendLine($"      <Beneficiario>{EscaparXml(pagamento.Beneficiario)}</Beneficiario>");
            xml.AppendLine($"      <ContaBeneficiario>{EscaparXml(pagamento.ContaBeneficiario)}</ContaBeneficiario>");
            xml.AppendLine($"      <BancoBeneficiario>{EscaparXml(pagamento.BancoBeneficiario)}</BancoBeneficiario>");
            
            if (!string.IsNullOrEmpty(pagamento.Referencia))
                xml.AppendLine($"      <Referencia>{EscaparXml(pagamento.Referencia)}</Referencia>");
            
            if (!string.IsNullOrEmpty(pagamento.Descricao))
                xml.AppendLine($"      <Descricao>{EscaparXml(pagamento.Descricao)}</Descricao>");
            
            xml.AppendLine($"      <Tipo>{pagamento.Tipo}</Tipo>");
            xml.AppendLine("    </Pagamento>");
        }

        xml.AppendLine("  </Pagamentos>");
        xml.AppendLine("</LotePagamentos>");

        File.WriteAllText(caminhoArquivo, xml.ToString(), Encoding.UTF8);
    }

    private string EscaparXml(string valor)
    {
        if (string.IsNullOrEmpty(valor))
            return "";

        return valor
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }
}
