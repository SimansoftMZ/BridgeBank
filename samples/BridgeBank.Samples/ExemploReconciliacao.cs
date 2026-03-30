using Simansoft.BridgeBank.Core;
using Simansoft.BridgeBank.Core.Strategies;
using Simansoft.BridgeBank.Core.Models;

namespace BridgeBank.Samples;

/// <summary>
/// Exemplo de reconciliação bancária utilizando transacções de um extracto
/// real e lançamentos ERP simulados (baseados nos extractos dos bancos parceiros).
/// </summary>
public static class ExemploReconciliacao
{
    public static void Executar(ExtratoBancario extrato)
    {
        Console.WriteLine("Reconciliação Bancária");
        Console.WriteLine(new string('-', 50));
        Console.WriteLine();

        List<Transacao> transacoes = extrato.Transacoes;
        List<LancamentoERP> lancamentos = CriarLancamentosERP();

        Console.WriteLine($"Extracto:              {extrato.Banco}");
        Console.WriteLine($"Transacções bancárias: {transacoes.Count}");
        Console.WriteLine($"Lançamentos ERP:       {lancamentos.Count}");
        Console.WriteLine();

        // Configurar motor com as 3 estratégias
        MotorReconciliacao motor = new();
        motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorReferencia());
        motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorValorEData(toleranciaDias: 3));
        motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorDescricao());

        List<ResultadoReconciliacao> resultados = motor.Reconciliar(transacoes, lancamentos);

        // Mostrar resultados
        Console.WriteLine("Resultados:");
        Console.WriteLine(new string('-', 100));
        Console.WriteLine($"{"ID",-38} {"Data",-12} {"Valor",14}  {"Estado",-40}");
        Console.WriteLine(new string('-', 100));

        foreach (ResultadoReconciliacao r in resultados)
        {
            string status = r.TipoCorrespondencia != TipoCorrespondencia.Nenhuma
                ? $"[OK] {r.TipoCorrespondencia} ({r.NivelConfianca:P0})"
                : "[--] Não reconciliada";

            string id = r.Transacao.Id.Length > 36 ? r.Transacao.Id[..33] + "..." : r.Transacao.Id;
            Console.WriteLine($"  {id,-38} {r.Transacao.Data:dd/MM/yyyy}   {r.Transacao.Valor,14:N2}  {status}");

            if (r.LancamentoCorrespondente != null)
                Console.WriteLine($"         -> {r.LancamentoCorrespondente.Id} ({r.LancamentoCorrespondente.Descricao})");
        }

        Console.WriteLine(new string('-', 100));

        // Estatísticas
        int reconciliadas = resultados.Count(r => r.TipoCorrespondencia != TipoCorrespondencia.Nenhuma);
        int porReferencia = resultados.Count(r => r.TipoCorrespondencia == TipoCorrespondencia.PorReferencia);
        int porValorData = resultados.Count(r => r.TipoCorrespondencia == TipoCorrespondencia.PorValorEData);
        int porDescricao = resultados.Count(r => r.TipoCorrespondencia == TipoCorrespondencia.PorDescricao);

        Console.WriteLine();
        Console.WriteLine($"Total:               {resultados.Count}");
        Console.WriteLine($"Reconciliadas:       {reconciliadas}");
        Console.WriteLine($"  Por referência:    {porReferencia}");
        Console.WriteLine($"  Por valor/data:    {porValorData}");
        Console.WriteLine($"  Por descrição:     {porDescricao}");
        Console.WriteLine($"Não encontradas:     {resultados.Count - reconciliadas}");

        if (resultados.Count > 0)
            Console.WriteLine($"Taxa de sucesso:     {(reconciliadas * 100.0 / resultados.Count):F1}%");

        // Lançamentos ERP não utilizados
        var idsUsados = resultados
            .Where(r => r.LancamentoCorrespondente != null)
            .Select(r => r.LancamentoCorrespondente!.Id)
            .ToHashSet();

        var naoUtilizados = lancamentos.Where(l => !idsUsados.Contains(l.Id)).ToList();

        if (naoUtilizados.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine($"Lançamentos ERP sem correspondência ({naoUtilizados.Count}):");
            foreach (var l in naoUtilizados)
                Console.WriteLine($"  {l.Id,-10} {l.Data:dd/MM/yyyy}   {l.Valor,14:N2}  {l.Descricao}");
        }
    }

    /// <summary>
    /// Lançamentos ERP simulados baseados nos extractos reais dos bancos parceiros.
    /// Incluem registos de diferentes bancos, valores que podem ou não coincidir
    /// com o extracto carregado, e cenários comuns do dia-a-dia empresarial.
    /// </summary>
    private static List<LancamentoERP> CriarLancamentosERP() =>
    [
        // ── Lançamentos baseados em transacções NedBank ──────────────
        // Depósito numerário — coincide com ref IW591333NBE5 (NedBank)
        new()
        {
            Id = "ERP-001",
            Data = new DateTime(2025, 12, 1),
            Valor = 4_400.00m,
            Descricao = "Depósito numerário recebido",
            Referencia = "IW591333NBE5",
            Cliente = "Diana Noelia Nhantumbo",
            Status = StatusLancamento.Pendente
        },

        // ── Lançamentos baseados em transacções Standard Bank ────────
        // Crédito Standard Bank — coincide por valor e data
        new()
        {
            Id = "ERP-002",
            Data = new DateTime(2025, 12, 15),
            Valor = 3_180.00m,
            Descricao = "Recebimento cliente — transferência bancária",
            NumeroDocumento = "REC-2025-0412",
            Cliente = "Cervejas de Moçambique SA",
            Status = StatusLancamento.Pendente
        },

        // ── Lançamentos baseados em transacções Millennium BIM ───────
        // Crédito entidade 14029 — coincide por valor e data
        new()
        {
            Id = "ERP-003",
            Data = new DateTime(2025, 12, 16),
            Valor = 132_829.99m,
            Descricao = "Crédito entidade 14029",
            NumeroDocumento = "CR-2025-0891",
            Cliente = "Entidade 14029",
            Status = StatusLancamento.Pendente
        },

        // ── Lançamentos baseados em transacções BCI ──────────────────
        // Pagamento de serviços — coincide por descrição ("Pag. Serv.")
        new()
        {
            Id = "ERP-004",
            Data = new DateTime(2025, 12, 1),
            Valor = 4_400.00m,
            Descricao = "Pagamento serviços recebido via BCI",
            Referencia = "PSERV-2025-001",
            Cliente = "Cliente BCI",
            Status = StatusLancamento.Pendente
        },

        // Comissão bancária BCI
        new()
        {
            Id = "ERP-005",
            Data = new DateTime(2025, 12, 1),
            Valor = 50.00m,
            Descricao = "Comissão bancária BCI",
            Fornecedor = "BCI",
            Status = StatusLancamento.Pendente
        },

        // ── Lançamentos baseados em transacções Access Bank ──────────
        // Depósito Access Bank — coincide por descrição ("DEPÓSITO")
        new()
        {
            Id = "ERP-006",
            Data = new DateTime(2025, 12, 1),
            Valor = 4_400.00m,
            Descricao = "Depósito recebido Access Bank",
            NumeroDocumento = "DEP-2025-0033",
            Cliente = "Depositante Access Bank",
            Status = StatusLancamento.Pendente
        },

        // ── Lançamentos baseados em transacções M-Pesa ───────────────
        // Pagamento M-Pesa — coincide por referência (M-Pesa tem sempre ref)
        new()
        {
            Id = "ERP-007",
            Data = new DateTime(2025, 12, 5),
            Valor = 1_500.00m,
            Descricao = "Recebimento via M-Pesa",
            Referencia = "MPESA-720457-001",
            Cliente = "Cliente M-Pesa",
            Status = StatusLancamento.Pendente
        },

        // ── Lançamentos ERP sem correspondência no extracto ──────────
        // Pagamento a fornecedor — registado no ERP mas ainda não saiu do banco
        new()
        {
            Id = "ERP-008",
            Data = new DateTime(2025, 12, 10),
            Valor = 75_000.00m,
            Descricao = "Pagamento fornecedor de materiais de construção",
            Referencia = "FAT-2025-1234",
            Fornecedor = "Cimentos de Moçambique",
            NumeroDocumento = "PAG-2025-0567",
            Status = StatusLancamento.Pendente
        },

        // Salários — processado no ERP mas ainda pendente no banco
        new()
        {
            Id = "ERP-009",
            Data = new DateTime(2025, 12, 20),
            Valor = 450_000.00m,
            Descricao = "Processamento salarial Dezembro 2025",
            Fornecedor = "Folha Salarial",
            NumeroDocumento = "SAL-2025-12",
            Status = StatusLancamento.Pendente
        },

        // Pagamento de renda — outro banco (FNB)
        new()
        {
            Id = "ERP-010",
            Data = new DateTime(2025, 12, 3),
            Valor = 85_000.00m,
            Descricao = "Renda mensal escritório Maputo",
            Referencia = "RENDA-2025-12",
            Fornecedor = "Imobiliária Polana Lda",
            NumeroDocumento = "PAG-2025-0590",
            Status = StatusLancamento.Pendente
        },

        // Pagamento de seguro — outro banco (Absa)
        new()
        {
            Id = "ERP-011",
            Data = new DateTime(2025, 12, 5),
            Valor = 22_500.00m,
            Descricao = "Seguro empresarial — apólice anual",
            Referencia = "SEG-2025-AP01",
            Fornecedor = "Hollard Seguros",
            NumeroDocumento = "PAG-2025-0601",
            Status = StatusLancamento.Pendente
        },

        // Pagamento de electricidade — conta de outro banco
        new()
        {
            Id = "ERP-012",
            Data = new DateTime(2025, 12, 12),
            Valor = 18_750.00m,
            Descricao = "Electricidade sede — EDM factura Dezembro",
            Referencia = "EDM-2025-12-4401",
            Fornecedor = "EDM - Electricidade de Moçambique",
            Status = StatusLancamento.Pendente
        },

        // Recebimento futuro — factura emitida mas pagamento ainda não recebido
        new()
        {
            Id = "ERP-013",
            Data = new DateTime(2025, 12, 18),
            Valor = 250_000.00m,
            Descricao = "Factura serviços de consultoria — cliente governo",
            Referencia = "FAT-2025-0089",
            Cliente = "Ministério das Obras Públicas",
            NumeroDocumento = "FAT-2025-0089",
            Status = StatusLancamento.Pendente
        },

        // Adiantamento a fornecedor — cheque ainda não descontado
        new()
        {
            Id = "ERP-014",
            Data = new DateTime(2025, 12, 8),
            Valor = 35_000.00m,
            Descricao = "Adiantamento fornecedor — compra de equipamento",
            Referencia = "ADI-2025-0044",
            Fornecedor = "TechnoMZ Equipamentos",
            NumeroDocumento = "PAG-2025-0615",
            Status = StatusLancamento.Pendente
        },
    ];
}
