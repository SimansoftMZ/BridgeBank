using BridgeBank.Core.Models;
using BridgeBank.Parsers.Excel;
using OfficeOpenXml;

namespace BridgeBank.Parsers.Bancos;

/// <summary>
/// Leitor de extratos do Millennium BIM
/// </summary>
public class LeitorExtratoMillenniumBIM : LeitorExcelBase
{
    private const int LinhaInicioTransacoes = 8;
    private const int ColunaData = 1;
    private const int ColunaDescricao = 3;
    private const int ColunaReferencia = 2;
    private const int ColunaValor = 4;
    private const int ColunaSaldo = 5;

    public override ExtratoBancario LerExtrato(string caminhoArquivo)
    {
        using var package = new ExcelPackage(new FileInfo(caminhoArquivo));
        var planilha = package.Workbook.Worksheets[0];

        var extrato = new ExtratoBancario
        {
            Banco = "Millennium BIM",
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

        extrato.SaldoFinal = ObterSaldoFinal(planilha);

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
        var valor = planilha.Cells[linha, ColunaValor].Value;
        return valor != null && decimal.TryParse(valor.ToString(), out var valorDecimal)
            ? Math.Abs(valorDecimal)
            : 0;
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
        var valor = planilha.Cells[linha, ColunaValor].Value;
        if (valor != null && decimal.TryParse(valor.ToString(), out var valorDecimal))
        {
            return valorDecimal >= 0 ? TipoTransacao.Credito : TipoTransacao.Debito;
        }
        return TipoTransacao.Debito;
    }

    private string ObterNumeroConta(ExcelWorksheet planilha)
    {
        return planilha.Cells[2, 2].Value?.ToString() ?? "N/A";
    }

    private DateTime ObterDataInicio(ExcelWorksheet planilha)
    {
        var valor = planilha.Cells[4, 2].Value;
        if (valor is DateTime data)
            return data;
        if (valor != null && DateTime.TryParse(valor.ToString(), out var dataParsed))
            return dataParsed;
        return DateTime.Now;
    }

    private DateTime ObterDataFim(ExcelWorksheet planilha)
    {
        var valor = planilha.Cells[5, 2].Value;
        if (valor is DateTime data)
            return data;
        if (valor != null && DateTime.TryParse(valor.ToString(), out var dataParsed))
            return dataParsed;
        return DateTime.Now;
    }

    private decimal ObterSaldoInicial(ExcelWorksheet planilha)
    {
        var valor = planilha.Cells[6, 2].Value;
        return valor != null && decimal.TryParse(valor.ToString(), out var saldo) ? saldo : 0;
    }

    private decimal ObterSaldoFinal(ExcelWorksheet planilha)
    {
        var ultimaLinha = planilha.Dimension.End.Row;
        for (int i = ultimaLinha; i >= LinhaInicioTransacoes; i--)
        {
            var saldo = planilha.Cells[i, ColunaSaldo].Value;
            if (saldo != null && decimal.TryParse(saldo.ToString(), out var valorSaldo))
                return valorSaldo;
        }
        return 0;
    }

    private bool LinhaVazia(ExcelWorksheet planilha, int linha)
    {
        return planilha.Cells[linha, ColunaData].Value == null;
    }
}
