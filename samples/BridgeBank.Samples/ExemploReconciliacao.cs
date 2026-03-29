using BridgeBank.Core;
using BridgeBank.Core.Strategies;
using Simansoft.BridgeBank.Core.Models;

namespace BridgeBank.Samples;

/// <summary>
/// Exemplo de reconciliação bancária com dados simulados.
/// </summary>
public static class ExemploReconciliacao
{
    public static void Executar()
    {
        Console.WriteLine("Reconciliação Bancária");
        Console.WriteLine(new string('-', 50));
        Console.WriteLine();

        var transacoes = CriarTransacoesSimuladas();
        var lancamentos = CriarLancamentosSimulados();

        Console.WriteLine($"Transacções bancárias: {transacoes.Count}");
        Console.WriteLine($"Lançamentos ERP:       {lancamentos.Count}");
        Console.WriteLine();

        // Configurar motor com as 3 estratégias
        var motor = new MotorReconciliacao();
        motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorReferencia());
        motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorValorEData(toleranciaDias: 3));
        motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorDescricao());

        var resultados = motor.Reconciliar(transacoes, lancamentos);

        // Mostrar resultados
        Console.WriteLine("Resultados:");
        Console.WriteLine(new string('-', 80));

        foreach (var r in resultados)
        {
            var status = r.TipoCorrespondencia != TipoCorrespondencia.Nenhuma
                ? $"[OK] {r.TipoCorrespondencia} ({r.NivelConfianca:P0})"
                : "[--] Não reconciliada";

            Console.WriteLine($"  {r.Transacao.Id}  {r.Transacao.Data:dd/MM}  {r.Transacao.Valor,12:N2}  {status}");

            if (r.LancamentoCorrespondente != null)
                Console.WriteLine($"         -> {r.LancamentoCorrespondente.Id} ({r.LancamentoCorrespondente.Descricao})");
        }

        Console.WriteLine(new string('-', 80));

        // Estatísticas
        var reconciliadas = resultados.Count(r => r.TipoCorrespondencia != TipoCorrespondencia.Nenhuma);
        Console.WriteLine();
        Console.WriteLine($"Total:           {resultados.Count}");
        Console.WriteLine($"Reconciliadas:   {reconciliadas}");
        Console.WriteLine($"Não encontradas: {resultados.Count - reconciliadas}");
        Console.WriteLine($"Taxa de sucesso: {(reconciliadas * 100.0 / resultados.Count):F1}%");
    }

    private static List<Transacao> CriarTransacoesSimuladas() =>
    [
        new()
        {
            Id = "T001",
            Data = new DateTime(2026, 1, 5),
            Valor = 5000.00m,
            Descricao = "Pagamento Fornecedor ABC",
            Referencia = "INV-2026-001",
            Tipo = TipoTransacao.Debito
        },
        new()
        {
            Id = "T002",
            Data = new DateTime(2026, 1, 6),
            Valor = 12500.50m,
            Descricao = "Transferência Cliente XYZ",
            Tipo = TipoTransacao.Credito
        },
        new()
        {
            Id = "T003",
            Data = new DateTime(2026, 1, 7),
            Valor = 3200.00m,
            Descricao = "Pagamento serviços telefonia",
            Referencia = "TEL-123",
            Tipo = TipoTransacao.Debito
        }
    ];

    private static List<LancamentoERP> CriarLancamentosSimulados() =>
    [
        new()
        {
            Id = "L001",
            Data = new DateTime(2026, 1, 5),
            Valor = 5000.00m,
            Descricao = "Factura Fornecedor ABC",
            Referencia = "INV-2026-001",
            Fornecedor = "ABC Lda",
            Status = StatusLancamento.Pendente
        },
        new()
        {
            Id = "L002",
            Data = new DateTime(2026, 1, 7),
            Valor = 3200.00m,
            Descricao = "Factura telefonia móvel",
            Referencia = "TEL-123",
            Fornecedor = "Vodacom",
            Status = StatusLancamento.Pendente
        },
        new()
        {
            Id = "L003",
            Data = new DateTime(2026, 1, 8),
            Valor = 12500.50m,
            Descricao = "Recebimento Cliente XYZ",
            Cliente = "XYZ SA",
            Status = StatusLancamento.Pendente
        }
    ];
}
