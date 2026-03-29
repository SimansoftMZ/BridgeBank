using System.Globalization;
using System.Text.RegularExpressions;
using BridgeBank.Core.Models;
using BridgeBank.Parsers.Excel;
using BridgeBank.Parsers.Util;
using NPOI.SS.UserModel;

namespace BridgeBank.Parsers.Bancos;

/// <summary>
/// Leitor de extratos do Standard Bank.
/// Formato: ficheiro .xlsx com todos os valores em texto.
/// </summary>
public partial class LeitorExtratoStandardBank : LeitorExcelBase
{
    // Estrutura (índices base 0):
    // Linha 0: "Standard Bank"
    // Linha 1: "Movimentos de Conta - {conta}"
    // Linha 2: Cabeçalhos: Data | Data valor | Descrição | Referência | Débito | Crédito | Saldo | Moeda
    // Linha 3+: Transacções (strings)
    // Linha N: "Copyright © ..."

    private const int LinhaInfoConta = 1;
    private const int LinhaInicioTransacoes = 3;

    private const int ColunaData = 0;
    private const int ColunaDescricao = 2;
    private const int ColunaReferencia = 3;
    private const int ColunaDebito = 4;
    private const int ColunaCredito = 5;
    private const int ColunaSaldo = 6;

    public override ExtratoBancario LerExtrato(string caminhoArquivo)
    {
        using var workbook = AbrirFicheiro(caminhoArquivo);
        var folha = workbook.GetSheetAt(0);
        var ultimaLinha = ObterUltimaLinha(folha);
        int ultimaLinhaTransacoesReal = LinhaInicioTransacoes;

        var extrato = new ExtratoBancario
        {
            Banco = "Standard Bank",
            NumeroConta = ObterNumeroConta(folha),
            Transacoes = new List<Transacao>()
        };

        for (int linha = LinhaInicioTransacoes; linha <= ultimaLinha; linha++)
        {
            var textoData = ObterTextoCelula(folha, linha, ColunaData);

            if (string.IsNullOrWhiteSpace(textoData))
                break;
            if (textoData.StartsWith("Copyright", StringComparison.OrdinalIgnoreCase))
                break;

            var transacao = ExtrairTransacao(folha, linha, textoData);
            if (transacao != null)
            {
                extrato.Transacoes.Add(transacao);
                ultimaLinhaTransacoesReal = linha;
            }
        }

        if (extrato.Transacoes.Count > 0)
        {
            extrato.DataInicio = extrato.Transacoes.Min(t => t.Data);
            extrato.DataFim = extrato.Transacoes.Max(t => t.Data);

            extrato.SaldoFinal = ParseadorNumerico.ParsearValorMonetario(
                ObterTextoCelula(folha, LinhaInicioTransacoes, ColunaSaldo));

            var saldoUltimaTransacao = ParseadorNumerico.ParsearValorMonetario(
                ObterTextoCelula(folha, ultimaLinhaTransacoesReal, ColunaSaldo));
            var ultimaTransacao = extrato.Transacoes[^1];
            extrato.SaldoInicial = saldoUltimaTransacao
                - (ultimaTransacao.Tipo == TipoTransacao.Credito ? ultimaTransacao.Valor : -ultimaTransacao.Valor);
        }

        return extrato;
    }

    private static Transacao? ExtrairTransacao(ISheet folha, int linha, string textoData)
    {
        if (!DateTime.TryParseExact(textoData, ["dd-MM-yyyy", "dd/MM/yyyy"],
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var data))
            return null;

        var textoDebito = ObterTextoCelula(folha, linha, ColunaDebito);
        var textoCredito = ObterTextoCelula(folha, linha, ColunaCredito);
        var isDebito = !string.IsNullOrEmpty(textoDebito);

        var valor = isDebito
            ? ParseadorNumerico.ParsearValorMonetario(textoDebito)
            : ParseadorNumerico.ParsearValorMonetario(textoCredito);

        return new Transacao
        {
            Id = Guid.NewGuid().ToString(),
            Data = data,
            Valor = valor,
            Descricao = ObterTextoCelula(folha, linha, ColunaDescricao),
            Referencia = ObterTextoCelula(folha, linha, ColunaReferencia) is { Length: > 0 } r ? r : null,
            Tipo = isDebito ? TipoTransacao.Debito : TipoTransacao.Credito
        };
    }

    private static string ObterNumeroConta(ISheet folha)
    {
        var texto = ObterTextoCelula(folha, LinhaInfoConta, 0);
        var match = ContaRegex().Match(texto);
        return match.Success ? match.Groups["conta"].Value : "N/A";
    }

    [GeneratedRegex(@"-\s*(?<conta>\d+)")]
    private static partial Regex ContaRegex();
}
