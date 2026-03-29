using System.Globalization;
using BridgeBank.Parsers.Excel;
using NPOI.SS.UserModel;
using Simansoft.BridgeBank.Core.Models;

namespace BridgeBank.Parsers.Bancos;

/// <summary>
/// Leitor de extratos do NedBank (Nedbank Moçambique).
/// Formato: ficheiro .xlsx com montantes numéricos e datas em texto.
/// </summary>
public class LeitorExtratoNedBank : LeitorExcelBase
{
    // Estrutura (índices base 0):
    // Linha 0: Cabeçalhos
    // Linha 1: "Saldo De Abertura" | ... | ... | ... | ... | {saldo} (numérico)
    // Linha 2+: Transacções (Data=String, Montante=numérico, Saldo=numérico)
    // Última linha: "Saldo Final" | ... | ... | ... | ... | {saldo} (numérico)

    private const int LinhaSaldoAbertura = 1;
    private const int LinhaInicioTransacoes = 2;

    private const int ColunaData = 0;
    private const int ColunaReferencia = 2;
    private const int ColunaDescricao = 3;
    private const int ColunaMontante = 4;
    private const int ColunaSaldo = 5;

    public override ExtratoBancario LerExtrato(string caminhoArquivo)
    {
        using var workbook = AbrirFicheiro(caminhoArquivo);
        var folha = workbook.GetSheetAt(0);
        var ultimaLinha = ObterUltimaLinha(folha);

        var extrato = new ExtratoBancario
        {
            Banco = "NedBank",
            NumeroConta = ObterNumeroConta(folha),
            SaldoInicial = (decimal)ObterNumericoCelula(folha, LinhaSaldoAbertura, ColunaSaldo),
            Transacoes = new List<Transacao>()
        };

        for (int linha = LinhaInicioTransacoes; linha <= ultimaLinha; linha++)
        {
            var textoData = ObterTextoCelula(folha, linha, ColunaData);

            if (textoData.StartsWith("Saldo", StringComparison.OrdinalIgnoreCase))
            {
                extrato.SaldoFinal = (decimal)ObterNumericoCelula(folha, linha, ColunaSaldo);
                break;
            }

            if (string.IsNullOrWhiteSpace(textoData))
                continue;

            var transacao = ExtrairTransacao(folha, linha, textoData);
            if (transacao != null)
                extrato.Transacoes.Add(transacao);
        }

        if (extrato.Transacoes.Count > 0)
        {
            extrato.DataInicio = extrato.Transacoes.Min(t => t.Data);
            extrato.DataFim = extrato.Transacoes.Max(t => t.Data);
        }

        return extrato;
    }

    private static Transacao? ExtrairTransacao(ISheet folha, int linha, string textoData)
    {
        if (!DateTime.TryParseExact(textoData, ["dd/MM/yyyy", "dd-MM-yyyy"],
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var data))
            return null;

        var montante = ObterNumericoCelula(folha, linha, ColunaMontante);

        return new Transacao
        {
            Id = Guid.NewGuid().ToString(),
            Data = data,
            Valor = (decimal)Math.Abs(montante),
            Descricao = ObterTextoCelula(folha, linha, ColunaDescricao),
            Referencia = ObterTextoCelula(folha, linha, ColunaReferencia) is { Length: > 0 } r ? r : null,
            Tipo = montante >= 0 ? TipoTransacao.Credito : TipoTransacao.Debito
        };
    }

    private static string ObterNumeroConta(ISheet folha)
    {
        return folha.SheetName.Replace("statement", "").Trim();
    }
}
