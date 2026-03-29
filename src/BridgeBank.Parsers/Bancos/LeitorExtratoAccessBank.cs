using System.Globalization;
using System.Text.RegularExpressions;
using Simansoft.BridgeBank.Parsers.Util;
using NPOI.SS.UserModel;
using Simansoft.BridgeBank.Core.Models;
using Simansoft.BridgeBank.Parsers.Excel;

namespace Simansoft.BridgeBank.Parsers.Bancos;

/// <summary>
/// Leitor de extratos do Access Bank Moçambique.
/// Formato: ficheiro .xls com valores em texto no formato numérico português.
/// </summary>
public partial class LeitorExtratoAccessBank : LeitorExcelBase
{
    // Estrutura (índices base 0):
    // Linha 2 (row 2), Col 6: "De {dataInicio} a {dataFim}"
    // Linha 3 (row 3), Col 7: {conta}
    // Linha 5 (row 5), Col 7: Saldo Abertura (string PT)
    // Linha 6 (row 6), Col 7: Saldo Fecho (string PT)
    // Linha 9 (row 9): Cabeçalhos
    // Linha 10+ (row 10+): Col0=Data | Col3=NºTransacção | Col5=Descrição | Col6=Valor | Col7=Saldo

    private const int LinhaMetaDatas = 2;
    private const int LinhaMetaConta = 3;
    private const int LinhaMetaSaldoAbertura = 5;
    private const int LinhaMetaSaldoFecho = 6;
    private const int LinhaInicioTransacoes = 10;

    private const int ColunaData = 0;
    private const int ColunaNumeroTransacao = 3;
    private const int ColunaDescricao = 5;
    private const int ColunaValor = 6;

    public override ExtratoBancario LerExtrato(string caminhoArquivo)
    {
        using IWorkbook workbook = AbrirFicheiro(caminhoArquivo);
        ISheet folha = workbook.GetSheetAt(0);

        (DateTime dataInicio, DateTime dataFim) = ExtrairDatas(folha);

        ExtratoBancario extrato = new()
        {
            Banco = "Access Bank",
            NumeroConta = ObterTextoCelula(folha, LinhaMetaConta, 7),
            DataInicio = dataInicio,
            DataFim = dataFim,
            SaldoInicial = ParseadorNumerico.ParsearValorMonetario(ObterTextoCelula(folha, LinhaMetaSaldoAbertura, 7)),
            SaldoFinal = ParseadorNumerico.ParsearValorMonetario(ObterTextoCelula(folha, LinhaMetaSaldoFecho, 7)),
            Transacoes = []
        };

        int ultimaLinha = ObterUltimaLinha(folha);
        for (int linha = LinhaInicioTransacoes; linha <= ultimaLinha; linha++)
        {
            if (CelulaVazia(folha, linha, ColunaData) && CelulaVazia(folha, linha, ColunaDescricao))
                break;

            Transacao? transacao = ExtrairTransacao(folha, linha);
            if (transacao != null)
                extrato.Transacoes.Add(transacao);
        }

        return extrato;
    }

    private static Transacao? ExtrairTransacao(ISheet folha, int linha)
    {
        string textoData = ObterTextoCelula(folha, linha, ColunaData);
        if (!TryParseData(textoData, out DateTime data))
            return null;

        string textoValor = ObterTextoCelula(folha, linha, ColunaValor);
        decimal valor = ParseadorNumerico.ParsearValorMonetario(textoValor);

        return new Transacao
        {
            Id = Guid.NewGuid().ToString(),
            Data = data,
            Valor = Math.Abs(valor),
            Descricao = ObterTextoCelula(folha, linha, ColunaDescricao),
            Referencia = ObterTextoCelula(folha, linha, ColunaNumeroTransacao),
            Tipo = valor < 0 ? TipoTransacao.Debito : TipoTransacao.Credito
        };
    }

    private static (DateTime dataInicio, DateTime dataFim) ExtrairDatas(ISheet folha)
    {
        string texto = ObterTextoCelula(folha, LinhaMetaDatas, 6);
        Match match = DatasRegex().Match(texto);
        if (match.Success)
        {
            DateTime.TryParseExact(match.Groups["inicio"].Value, "dd-MM-yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime inicio);
            DateTime.TryParseExact(match.Groups["fim"].Value, "dd-MM-yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fim);
            return (inicio, fim);
        }
        return (DateTime.MinValue, DateTime.MinValue);
    }

    private static bool TryParseData(string texto, out DateTime data)
    {
        string[] formatos = ["dd/MM/yyyy", "dd-MM-yyyy"];
        return DateTime.TryParseExact(texto, formatos, CultureInfo.InvariantCulture,
            DateTimeStyles.None, out data);
    }

    [GeneratedRegex(@"De\s+(?<inicio>\d{2}-\d{2}-\d{4})\s+a\s+(?<fim>\d{2}-\d{2}-\d{4})", RegexOptions.IgnoreCase)]
    private static partial Regex DatasRegex();
}