using BridgeBank.Core.Models;
using BridgeBank.Parsers.Interfaces;
using OfficeOpenXml;

namespace BridgeBank.Parsers.Excel;

/// <summary>
/// Leitor base para extratos em formato Excel
/// </summary>
public abstract class LeitorExcelBase : ILeitorExtrato
{
    protected LeitorExcelBase()
    {
    }

    public virtual bool SuportaArquivo(string caminhoArquivo)
    {
        var extensao = Path.GetExtension(caminhoArquivo).ToLowerInvariant();
        return extensao == ".xlsx" || extensao == ".xls";
    }

    public abstract ExtratoBancario LerExtrato(string caminhoArquivo);

    protected virtual Transacao ExtrairTransacao(ExcelWorksheet planilha, int linha)
    {
        return new Transacao
        {
            Id = Guid.NewGuid().ToString(),
            Data = ObterData(planilha, linha),
            Valor = ObterValor(planilha, linha),
            Descricao = ObterDescricao(planilha, linha),
            Referencia = ObterReferencia(planilha, linha),
            Tipo = ObterTipo(planilha, linha)
        };
    }

    protected abstract DateTime ObterData(ExcelWorksheet planilha, int linha);
    protected abstract decimal ObterValor(ExcelWorksheet planilha, int linha);
    protected abstract string ObterDescricao(ExcelWorksheet planilha, int linha);
    protected abstract string? ObterReferencia(ExcelWorksheet planilha, int linha);
    protected abstract TipoTransacao ObterTipo(ExcelWorksheet planilha, int linha);
}
