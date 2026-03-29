using System.Globalization;
using System.Text.RegularExpressions;
using BridgeBank.Core.Models;
using BridgeBank.Parsers.Excel;
using BridgeBank.Parsers.Util;
using NPOI.SS.UserModel;

namespace BridgeBank.Parsers.Bancos;

/// <summary>
/// Leitor de extratos do Millennium BIM.
/// Formato: ficheiro .xlsx com colunas deslocadas (início na coluna C/índice 2).
/// </summary>
public partial class LeitorExtratoMillenniumBIM : LeitorExcelBase
{
    // Estrutura do ficheiro Millennium BIM (índices base 0):
    // Linha 5 (row 5), Col C (2): "Lista de transacções da conta Nº {conta} em MZN, de {inicio} até {fim}"
    // Linha 6 (row 6), Col I (8): Saldo Inicial (string)
    // Linha 9 (row 9), Col I (8): Saldo Final (string)
    // Linha 10 (row 10): Cabeçalhos
    // Linha 11+ (row 11+): Col2=Data | Col4=Descrição | Col6=Débito(num) | Col8=Crédito(num) | Col9=Saldo(num)

    private const int LinhaInfoConta = 5;
    private const int LinhaSaldoInicial = 6;
    private const int LinhaSaldoFinal = 9;
    private const int LinhaInicioTransacoes = 11;

    private const int ColunaDataTransacao = 2;
    private const int ColunaDescricao = 4;
    private const int ColunaDebito = 6;
    private const int ColunaCredito = 8;

    public override ExtratoBancario LerExtrato(string caminhoArquivo)
    {
        using var workbook = AbrirFicheiro(caminhoArquivo);
        var folha = workbook.GetSheetAt(0);

        var (conta, dataInicio, dataFim) = ExtrairInfoConta(folha);

        var extrato = new ExtratoBancario
        {
            Banco = "Millennium BIM",
            NumeroConta = conta,
            DataInicio = dataInicio,
            DataFim = dataFim,
            SaldoInicial = ParseadorNumerico.ParsearValorMonetario(ObterTextoCelula(folha, LinhaSaldoInicial, 8)),
            SaldoFinal = ParseadorNumerico.ParsearValorMonetario(ObterTextoCelula(folha, LinhaSaldoFinal, 8)),
            Transacoes = new List<Transacao>()
        };

        var ultimaLinha = ObterUltimaLinha(folha);
        for (int linha = LinhaInicioTransacoes; linha <= ultimaLinha; linha++)
        {
            if (CelulaVazia(folha, linha, ColunaDataTransacao))
                continue;

            var transacao = ExtrairTransacao(folha, linha);
            if (transacao != null)
                extrato.Transacoes.Add(transacao);
        }

        return extrato;
    }

    private static Transacao? ExtrairTransacao(ISheet folha, int linha)
    {
        var data = ObterDataCelula(folha, linha, ColunaDataTransacao);
        if (data == null)
        {
            var textoData = ObterTextoCelula(folha, linha, ColunaDataTransacao);
            if (DateTime.TryParseExact(textoData, ["dd/MM/yyyy", "dd-MM-yyyy"],
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                data = parsed;
        }

        if (data == null)
            return null;

        var debito = ObterNumericoCelula(folha, linha, ColunaDebito);
        var credito = ObterNumericoCelula(folha, linha, ColunaCredito);
        var isCredito = credito > 0;
        var valor = isCredito ? credito : Math.Abs(debito);

        return new Transacao
        {
            Id = Guid.NewGuid().ToString(),
            Data = data.Value,
            Valor = (decimal)valor,
            Descricao = ObterTextoCelula(folha, linha, ColunaDescricao),
            Tipo = isCredito ? TipoTransacao.Credito : TipoTransacao.Debito
        };
    }

    private static (string conta, DateTime dataInicio, DateTime dataFim) ExtrairInfoConta(ISheet folha)
    {
        var texto = ObterTextoCelula(folha, LinhaInfoConta, 2);
        var match = InfoContaRegex().Match(texto);
        if (match.Success)
        {
            var conta = match.Groups["conta"].Value;
            DateTime.TryParseExact(match.Groups["inicio"].Value, "dd-MM-yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var inicio);
            DateTime.TryParseExact(match.Groups["fim"].Value, "dd-MM-yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var fim);
            return (conta, inicio, fim);
        }
        return ("N/A", DateTime.MinValue, DateTime.MinValue);
    }

    [GeneratedRegex(@"conta\s+N[ºo°]\s*(?<conta>\d+).*?de\s+(?<inicio>\d{2}-\d{2}-\d{4})\s+at[ée]\s+(?<fim>\d{2}-\d{2}-\d{4})", RegexOptions.IgnoreCase)]
    private static partial Regex InfoContaRegex();
}
