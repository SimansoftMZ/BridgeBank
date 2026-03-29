using System.Globalization;
using Simansoft.BridgeBank.Parsers.Util;
using NPOI.SS.UserModel;
using Simansoft.BridgeBank.Core.Models;
using Simansoft.BridgeBank.Parsers.Excel;

namespace Simansoft.BridgeBank.Parsers.Bancos;

/// <summary>
/// Leitor de extratos do BCI (Banco Comercial e de Investimentos).
/// Formato: ficheiro .xls com estrutura de colunas fixa.
/// </summary>
public class LeitorExtratoBCI : LeitorExcelBase
{
    // Estrutura do ficheiro BCI (índices base 0):
    // Linha 1 (row 1): Conta | {número}
    // Linha 3 (row 3): Saldo Anterior | {valor} MZN
    // Linha 8 (row 8): Cabeçalhos
    // Linha 9+ (row 9+): Data Mov. | Nº Doc | Num. Oper. | Data Valor | Descrição | Valor (+/-) | Saldo | Moeda

    private const int LinhaConta = 1;
    private const int LinhaSaldoAnterior = 3;
    private const int LinhaInicioTransacoes = 9;

    private const int ColunaDataMov = 0;
    private const int ColunaNumeroDocumento = 1;
    private const int ColunaDescricao = 4;
    private const int ColunaValor = 5;
    private const int ColunaSaldo = 6;

    public override ExtratoBancario LerExtrato(string caminhoArquivo)
    {
        using IWorkbook workbook = AbrirFicheiro(caminhoArquivo);
        ISheet folha = workbook.GetSheetAt(0);

        ExtratoBancario extrato = new()
        {
            Banco = "BCI",
            NumeroConta = ObterTextoCelula(folha, LinhaConta, 1),
            SaldoInicial = ParseadorNumerico.ParsearValorMonetario(ObterTextoCelula(folha, LinhaSaldoAnterior, 1)),
            Transacoes = []
        };

        int ultimaLinha = ObterUltimaLinha(folha);
        for (int linha = LinhaInicioTransacoes; linha <= ultimaLinha; linha++)
        {
            if (CelulaVazia(folha, linha, ColunaDataMov) && CelulaVazia(folha, linha, ColunaDescricao))
                break;

            Transacao? transacao = ExtrairTransacao(folha, linha);
            if (transacao != null)
                extrato.Transacoes.Add(transacao);
        }

        if (extrato.Transacoes.Count > 0)
        {
            extrato.DataInicio = extrato.Transacoes.Min(t => t.Data);
            extrato.DataFim = extrato.Transacoes.Max(t => t.Data);
        }

        extrato.SaldoFinal = ObterSaldoFinal(folha, ultimaLinha, extrato.SaldoInicial, extrato.Transacoes);

        return extrato;
    }

    private static Transacao? ExtrairTransacao(ISheet folha, int linha)
    {
        string textoData = ObterTextoCelula(folha, linha, ColunaDataMov);
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
            Referencia = ObterTextoCelula(folha, linha, ColunaNumeroDocumento),
            DocumentoOrigem = ObterTextoCelula(folha, linha, ColunaNumeroDocumento),
            Tipo = valor < 0 ? TipoTransacao.Debito : TipoTransacao.Credito
        };
    }

    private static decimal ObterSaldoFinal(ISheet folha, int ultimaLinha, decimal saldoInicial, List<Transacao> transacoes)
    {
        for (int i = ultimaLinha; i >= LinhaInicioTransacoes; i--)
        {
            string texto = ObterTextoCelula(folha, i, ColunaSaldo);
            if (!string.IsNullOrEmpty(texto))
            {
                decimal saldo = ParseadorNumerico.ParsearValorMonetario(texto);
                if (saldo != 0) return saldo;
            }
        }

        decimal saldoCalculado = saldoInicial;
        foreach (Transacao t in transacoes)
            saldoCalculado += t.Tipo == TipoTransacao.Credito ? t.Valor : -t.Valor;
        return saldoCalculado;
    }

    private static bool TryParseData(string texto, out DateTime data)
    {
        string[] formatos = ["dd-MM-yyyy", "dd/MM/yyyy", "yyyy-MM-dd"];
        return DateTime.TryParseExact(texto, formatos, CultureInfo.InvariantCulture,
            DateTimeStyles.None, out data);
    }
}