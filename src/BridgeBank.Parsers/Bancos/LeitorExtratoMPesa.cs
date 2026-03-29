using System.Globalization;
using BridgeBank.Parsers.Excel;
using NPOI.SS.UserModel;
using Simansoft.BridgeBank.Core.Models;

namespace BridgeBank.Parsers.Bancos;

/// <summary>
/// Leitor de extratos do M-Pesa (Vodacom Moçambique).
/// Formato: ficheiro .xls com transacções agrupadas por recibo.
/// </summary>
public class LeitorExtratoMPesa : LeitorExcelBase
{
    // Estrutura (índices base 0):
    // Linha 0: "Account Holder:" | {nome}
    // Linha 1: "Short Code:" | {código}
    // Linha 3: "Time Period:" | "From" | {dataInicio} | "To" | {dataFim}
    // Linha 5: Cabeçalhos
    // Linha 6+: Col0=Receipt | Col1=CompletionTime | Col3=Details | Col4=Status
    //           Col6=PaidIn | Col7=Withdrawn | Col8=Balance | Col10=OppositeParty

    private const int LinhaMetaCodigo = 1;
    private const int LinhaMetaPeriodo = 3;
    private const int LinhaInicioTransacoes = 6;

    private const int ColunaRecibo = 0;
    private const int ColunaDataConclusao = 1;
    private const int ColunaDetalhes = 3;
    private const int ColunaStatus = 4;
    private const int ColunaPaidIn = 6;
    private const int ColunaWithdrawn = 7;
    private const int ColunaSaldo = 8;
    private const int ColunaParteOposta = 10;

    public override ExtratoBancario LerExtrato(string caminhoArquivo)
    {
        using var workbook = AbrirFicheiro(caminhoArquivo);
        var folha = workbook.GetSheetAt(0);
        var ultimaLinha = ObterUltimaLinha(folha);

        var (dataInicio, dataFim) = ExtrairPeriodo(folha);

        var extrato = new ExtratoBancario
        {
            Banco = "M-Pesa",
            NumeroConta = ObterTextoCelula(folha, LinhaMetaCodigo, 1),
            DataInicio = dataInicio,
            DataFim = dataFim,
            Transacoes = new List<Transacao>()
        };

        for (int linha = LinhaInicioTransacoes; linha <= ultimaLinha; linha++)
        {
            if (CelulaVazia(folha, linha, ColunaRecibo))
                continue;

            var status = ObterTextoCelula(folha, linha, ColunaStatus);
            if (!status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                continue;

            var paidIn = ObterNumericoCelula(folha, linha, ColunaPaidIn);
            var withdrawn = ObterNumericoCelula(folha, linha, ColunaWithdrawn);

            if (paidIn == 0 && withdrawn == 0)
                continue;

            var transacao = ExtrairTransacao(folha, linha, paidIn, withdrawn);
            if (transacao != null)
                extrato.Transacoes.Add(transacao);
        }

        if (extrato.Transacoes.Count > 0)
        {
            extrato.SaldoFinal = ObterPrimeiroSaldo(folha, ultimaLinha);
            extrato.SaldoInicial = ObterUltimoSaldo(folha, ultimaLinha);
        }

        return extrato;
    }

    private static Transacao? ExtrairTransacao(ISheet folha, int linha, double paidIn, double withdrawn)
    {
        var textoData = ObterTextoCelula(folha, linha, ColunaDataConclusao);
        if (!TryParseData(textoData, out var data))
            return null;

        var isCredito = paidIn > 0;
        var valor = isCredito ? paidIn : Math.Abs(withdrawn);

        return new Transacao
        {
            Id = Guid.NewGuid().ToString(),
            Data = data,
            Valor = (decimal)valor,
            Descricao = ObterTextoCelula(folha, linha, ColunaDetalhes),
            Referencia = ObterTextoCelula(folha, linha, ColunaRecibo),
            Tipo = isCredito ? TipoTransacao.Credito : TipoTransacao.Debito,
            Beneficiario = ObterTextoCelula(folha, linha, ColunaParteOposta)
        };
    }

    private static (DateTime inicio, DateTime fim) ExtrairPeriodo(ISheet folha)
    {
        var textoInicio = ObterTextoCelula(folha, LinhaMetaPeriodo, 2);
        var textoFim = ObterTextoCelula(folha, LinhaMetaPeriodo, 4);

        TryParseData(textoInicio, out var inicio);
        TryParseData(textoFim, out var fim);
        return (inicio, fim);
    }

    private static decimal ObterPrimeiroSaldo(ISheet folha, int ultimaLinha)
    {
        for (int i = LinhaInicioTransacoes; i <= ultimaLinha; i++)
        {
            var saldo = ObterNumericoCelula(folha, i, ColunaSaldo);
            if (saldo != 0) return (decimal)saldo;
        }
        return 0;
    }

    private static decimal ObterUltimoSaldo(ISheet folha, int ultimaLinha)
    {
        for (int i = ultimaLinha; i >= LinhaInicioTransacoes; i--)
        {
            var saldo = ObterNumericoCelula(folha, i, ColunaSaldo);
            if (saldo != 0) return (decimal)saldo;
        }
        return 0;
    }

    private static bool TryParseData(string texto, out DateTime data)
    {
        data = DateTime.MinValue;
        if (string.IsNullOrWhiteSpace(texto)) return false;

        string[] formatos = ["dd-MM-yyyy HH:mm:ss", "dd/MM/yyyy HH:mm:ss", "dd-MM-yyyy", "dd/MM/yyyy"];
        return DateTime.TryParseExact(texto, formatos, CultureInfo.InvariantCulture,
            DateTimeStyles.None, out data);
    }
}
