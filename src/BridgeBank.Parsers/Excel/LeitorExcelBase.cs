using System.Globalization;
using BridgeBank.Core.Models;
using BridgeBank.Parsers.Interfaces;
using NPOI.SS.UserModel;

namespace BridgeBank.Parsers.Excel;

/// <summary>
/// Leitor base para extratos em formato Excel (.xls e .xlsx) utilizando NPOI.
/// </summary>
public abstract class LeitorExcelBase : ILeitorExtrato
{
    public virtual bool SuportaArquivo(string caminhoArquivo)
    {
        var extensao = Path.GetExtension(caminhoArquivo).ToLowerInvariant();
        return extensao == ".xlsx" || extensao == ".xls";
    }

    public abstract ExtratoBancario LerExtrato(string caminhoArquivo);

    /// <summary>
    /// Abre um ficheiro Excel (.xls ou .xlsx) e retorna o workbook.
    /// </summary>
    protected static IWorkbook AbrirFicheiro(string caminhoArquivo)
    {
        using var stream = File.Open(caminhoArquivo, FileMode.Open, FileAccess.Read, FileShare.Read);
        return WorkbookFactory.Create(stream);
    }

    /// <summary>
    /// Obtém o texto de uma célula, retornando string vazia se estiver vazia ou nula.
    /// </summary>
    protected static string ObterTextoCelula(ISheet folha, int linha, int coluna)
    {
        var row = folha.GetRow(linha);
        if (row == null) return string.Empty;

        var cell = row.GetCell(coluna);
        if (cell == null) return string.Empty;

        return cell.CellType switch
        {
            CellType.String => cell.StringCellValue?.Trim() ?? string.Empty,
            CellType.Numeric => DateUtil.IsCellDateFormatted(cell)
                ? cell.DateCellValue?.ToString("dd/MM/yyyy") ?? string.Empty
                : cell.NumericCellValue.ToString(CultureInfo.InvariantCulture),
            CellType.Boolean => cell.BooleanCellValue.ToString(),
            CellType.Formula => ObterValorFormula(cell),
            _ => string.Empty
        };
    }

    /// <summary>
    /// Obtém o valor numérico de uma célula. Retorna 0 se estiver vazia.
    /// </summary>
    protected static double ObterNumericoCelula(ISheet folha, int linha, int coluna)
    {
        var row = folha.GetRow(linha);
        if (row == null) return 0;

        var cell = row.GetCell(coluna);
        if (cell == null) return 0;

        return cell.CellType switch
        {
            CellType.Numeric => cell.NumericCellValue,
            CellType.String => double.TryParse(cell.StringCellValue,
                NumberStyles.Any, CultureInfo.InvariantCulture, out var val) ? val : 0,
            CellType.Formula => cell.CachedFormulaResultType == CellType.Numeric
                ? cell.NumericCellValue : 0,
            _ => 0
        };
    }

    /// <summary>
    /// Obtém o valor DateTime de uma célula.
    /// </summary>
    protected static DateTime? ObterDataCelula(ISheet folha, int linha, int coluna)
    {
        var row = folha.GetRow(linha);
        if (row == null) return null;

        var cell = row.GetCell(coluna);
        if (cell == null) return null;

        if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
            return cell.DateCellValue;

        return null;
    }

    /// <summary>
    /// Verifica se uma célula está vazia.
    /// </summary>
    protected static bool CelulaVazia(ISheet folha, int linha, int coluna)
    {
        var row = folha.GetRow(linha);
        if (row == null) return true;

        var cell = row.GetCell(coluna);
        if (cell == null) return true;

        return cell.CellType == CellType.Blank
            || (cell.CellType == CellType.String && string.IsNullOrWhiteSpace(cell.StringCellValue));
    }

    /// <summary>
    /// Obtém o índice da última linha utilizada na folha.
    /// </summary>
    protected static int ObterUltimaLinha(ISheet folha)
    {
        return folha.LastRowNum;
    }

    private static string ObterValorFormula(ICell cell)
    {
        return cell.CachedFormulaResultType switch
        {
            CellType.String => cell.StringCellValue?.Trim() ?? string.Empty,
            CellType.Numeric => DateUtil.IsCellDateFormatted(cell)
                ? cell.DateCellValue?.ToString("dd/MM/yyyy") ?? string.Empty
                : cell.NumericCellValue.ToString(CultureInfo.InvariantCulture),
            _ => string.Empty
        };
    }
}
