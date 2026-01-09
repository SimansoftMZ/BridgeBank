using BridgeBank.Core.Models;
using BridgeBank.Parsers.Excel;
using OfficeOpenXml;

namespace BridgeBank.Parsers.Bancos;

/// <summary>
/// Leitor de extratos do BCI (Banco Comercial e de Investimentos)
/// </summary>
public class LeitorExtratoBCI : LeitorExcelBase
{
    private const int LinhaInicioTransacoes = 10;
    private const int ColunaData = 1;
    private const int ColunaDescricao = 2;
    private const int ColunaReferencia = 3;
    private const int ColunaDebito = 4;
    private const int ColunaCredito = 5;

    public override ExtratoBancario LerExtrato(string caminhoArquivo)
    {
        using var package = new ExcelPackage(new FileInfo(caminhoArquivo));
        var planilha = package.Workbook.Worksheets[0];

        var extrato = new ExtratoBancario
        {
            Banco = "BCI",
            NumeroConta = ObterNumeroConta(planilha),
            DataInicio = ObterDataInicio(planilha),
            DataFim = ObterDataFim(planilha),
            SaldoInicial = ObterSaldoInicial(planilha),
            Transacoes = new List<Transacao>()
        };

        for (int linha = LinhaInicioTransacoes; linha <= planilha.Dimension.End.Row; linha++)
        {
            if (LinhaVazia(planilha, linha))
                break;

            var transacao = ExtrairTransacao(planilha, linha);
            extrato.Transacoes.Add(transacao);
        }

        extrato.SaldoFinal = ObterSaldoFinal(planilha, extrato.SaldoInicial, extrato.Transacoes);

        return extrato;
    }

    protected override DateTime ObterData(ExcelWorksheet planilha, int linha)
    {
        var valor = planilha.Cells[linha, ColunaData].Value;
        if (valor is DateTime data)
            return data;
        if (valor != null && DateTime.TryParse(valor.ToString(), out var dataParsed))
            return dataParsed;
        return DateTime.MinValue;
    }

    protected override decimal ObterValor(ExcelWorksheet planilha, int linha)
    {
        var debito = planilha.Cells[linha, ColunaDebito].Value;
        var credito = planilha.Cells[linha, ColunaCredito].Value;

        if (debito != null && decimal.TryParse(debito.ToString(), out var valorDebito))
            return valorDebito;

        if (credito != null && decimal.TryParse(credito.ToString(), out var valorCredito))
            return valorCredito;

        return 0;
    }

    protected override string ObterDescricao(ExcelWorksheet planilha, int linha)
    {
        return planilha.Cells[linha, ColunaDescricao].Value?.ToString() ?? string.Empty;
    }

    protected override string? ObterReferencia(ExcelWorksheet planilha, int linha)
    {
        return planilha.Cells[linha, ColunaReferencia].Value?.ToString();
    }

    protected override TipoTransacao ObterTipo(ExcelWorksheet planilha, int linha)
    {
        var debito = planilha.Cells[linha, ColunaDebito].Value;
        return debito != null ? TipoTransacao.Debito : TipoTransacao.Credito;
    }

    private string ObterNumeroConta(ExcelWorksheet planilha)
    {
        return planilha.Cells[3, 2].Value?.ToString() ?? "N/A";
    }

    private DateTime ObterDataInicio(ExcelWorksheet planilha)
    {
        var valor = planilha.Cells[5, 2].Value;
        if (valor is DateTime data)
            return data;
        if (valor != null && DateTime.TryParse(valor.ToString(), out var dataParsed))
            return dataParsed;
        return DateTime.Now;
    }

    private DateTime ObterDataFim(ExcelWorksheet planilha)
    {
        var valor = planilha.Cells[6, 2].Value;
        if (valor is DateTime data)
            return data;
        if (valor != null && DateTime.TryParse(valor.ToString(), out var dataParsed))
            return dataParsed;
        return DateTime.Now;
    }

    private decimal ObterSaldoInicial(ExcelWorksheet planilha)
    {
        var valor = planilha.Cells[7, 2].Value;
        return valor != null && decimal.TryParse(valor.ToString(), out var saldo) ? saldo : 0;
    }

    private decimal ObterSaldoFinal(ExcelWorksheet planilha, decimal saldoInicial, List<Transacao> transacoes)
    {
        var saldo = saldoInicial;
        foreach (var transacao in transacoes)
        {
            saldo += transacao.Tipo == TipoTransacao.Credito ? transacao.Valor : -transacao.Valor;
        }
        return saldo;
    }

    private bool LinhaVazia(ExcelWorksheet planilha, int linha)
    {
        return planilha.Cells[linha, ColunaData].Value == null &&
               planilha.Cells[linha, ColunaDescricao].Value == null;
    }
}
