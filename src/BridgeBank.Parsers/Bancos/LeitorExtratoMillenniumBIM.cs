using System.Globalization;
using System.Text.RegularExpressions;
using Simansoft.BridgeBank.Parsers.Util;
using NPOI.SS.UserModel;
using Simansoft.BridgeBank.Core.Models;
using Simansoft.BridgeBank.Parsers.Excel;

namespace Simansoft.BridgeBank.Parsers.Bancos;

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
        using IWorkbook workbook = AbrirFicheiro(caminhoArquivo);
        ISheet folha = workbook.GetSheetAt(0);

        (string conta, DateTime dataInicio, DateTime dataFim) = ExtrairInfoConta(folha);

        ExtratoBancario extrato = new()
        {
            Banco = "Millennium BIM",
            NumeroConta = conta,
            DataInicio = dataInicio,
            DataFim = dataFim,
            SaldoInicial = ParseadorNumerico.ParsearValorMonetario(ObterTextoCelula(folha, LinhaSaldoInicial, 8)),
            SaldoFinal = ParseadorNumerico.ParsearValorMonetario(ObterTextoCelula(folha, LinhaSaldoFinal, 8)),
            Transacoes = []
        };

        int ultimaLinha = ObterUltimaLinha(folha);
        for (int linha = LinhaInicioTransacoes; linha <= ultimaLinha; linha++)
        {
            if (CelulaVazia(folha, linha, ColunaDataTransacao))
                continue;

            Transacao? transacao = ExtrairTransacao(folha, linha);
            if (transacao != null)
                extrato.Transacoes.Add(transacao);
        }

        return extrato;
    }

    private static Transacao? ExtrairTransacao(ISheet folha, int linha)
    {
        DateTime? data = ObterDataCelula(folha, linha, ColunaDataTransacao);
        if (data == null)
        {
            string textoData = ObterTextoCelula(folha, linha, ColunaDataTransacao);
            if (DateTime.TryParseExact(textoData, ["dd/MM/yyyy", "dd-MM-yyyy"],
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed))
                data = parsed;
        }

        if (data == null)
            return null;

        double debito = ObterNumericoCelula(folha, linha, ColunaDebito);
        double credito = ObterNumericoCelula(folha, linha, ColunaCredito);
        bool isCredito = credito > 0;
        double valor = isCredito ? credito : Math.Abs(debito);

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
        string texto = ObterTextoCelula(folha, LinhaInfoConta, 2);
        Match match = InfoContaRegex().Match(texto);
        if (match.Success)
        {
            string conta = match.Groups["conta"].Value;
            DateTime.TryParseExact(match.Groups["inicio"].Value, "dd-MM-yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime inicio);
            DateTime.TryParseExact(match.Groups["fim"].Value, "dd-MM-yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fim);
            return (conta, inicio, fim);
        }
        return ("N/A", DateTime.MinValue, DateTime.MinValue);
    }

    [GeneratedRegex(@"conta\s+N[ºo°]\s*(?<conta>\d+).*?de\s+(?<inicio>\d{2}-\d{2}-\d{4})\s+at[ée]\s+(?<fim>\d{2}-\d{2}-\d{4})", RegexOptions.IgnoreCase)]
    private static partial Regex InfoContaRegex();
}