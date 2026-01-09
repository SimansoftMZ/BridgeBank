using BridgeBank.Core;
using BridgeBank.Core.Models;
using BridgeBank.Core.Strategies;
using BridgeBank.Generators.Bancos;
using BridgeBank.Generators.Models;

namespace BridgeBank.Samples;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== BridgeBank - Exemplo de Reconciliação Bancária ===\n");

        // Exemplo 1: Reconciliação básica
        ExemploReconciliacaoBasica();

        Console.WriteLine("\n" + new string('-', 60) + "\n");

        // Exemplo 2: Geração de ficheiros de pagamento
        ExemploGeracaoFicheirosPagamento();
    }

    static void ExemploReconciliacaoBasica()
    {
        Console.WriteLine("📋 Exemplo 1: Reconciliação Bancária\n");

        // Criar transações bancárias simuladas
        var transacoes = new List<Transacao>
        {
            new Transacao
            {
                Id = "T001",
                Data = new DateTime(2026, 1, 5),
                Valor = 5000.00m,
                Descricao = "Pagamento Fornecedor ABC",
                Referencia = "INV-2026-001",
                Tipo = TipoTransacao.Debito
            },
            new Transacao
            {
                Id = "T002",
                Data = new DateTime(2026, 1, 6),
                Valor = 12500.50m,
                Descricao = "Transferência Cliente XYZ",
                Tipo = TipoTransacao.Credito
            },
            new Transacao
            {
                Id = "T003",
                Data = new DateTime(2026, 1, 7),
                Valor = 3200.00m,
                Descricao = "Pagamento serviços telefonia",
                Referencia = "TEL-123",
                Tipo = TipoTransacao.Debito
            }
        };

        // Criar lançamentos ERP simulados
        var lancamentos = new List<LancamentoERP>
        {
            new LancamentoERP
            {
                Id = "L001",
                Data = new DateTime(2026, 1, 5),
                Valor = 5000.00m,
                Descricao = "Fatura Fornecedor ABC",
                Referencia = "INV-2026-001",
                Fornecedor = "ABC Lda",
                Status = StatusLancamento.Pendente
            },
            new LancamentoERP
            {
                Id = "L002",
                Data = new DateTime(2026, 1, 7),
                Valor = 3200.00m,
                Descricao = "Fatura telefonia móvel",
                Referencia = "TEL-123",
                Fornecedor = "Vodacom",
                Status = StatusLancamento.Pendente
            },
            new LancamentoERP
            {
                Id = "L003",
                Data = new DateTime(2026, 1, 8),
                Valor = 12500.50m,
                Descricao = "Recebimento Cliente XYZ",
                Cliente = "XYZ SA",
                Status = StatusLancamento.Pendente
            }
        };

        // Criar motor de reconciliação
        var motor = new MotorReconciliacao();
        motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorReferencia());
        motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorValorEData(toleranciaDias: 3));
        motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorDescricao());

        // Executar reconciliação
        var resultados = motor.Reconciliar(transacoes, lancamentos);

        // Mostrar resultados
        Console.WriteLine("Resultados da Reconciliação:\n");
        foreach (var resultado in resultados)
        {
            Console.WriteLine($"Transação: {resultado.Transacao.Id}");
            Console.WriteLine($"  Data: {resultado.Transacao.Data:dd/MM/yyyy}");
            Console.WriteLine($"  Valor: {resultado.Transacao.Valor:C} MZN");
            Console.WriteLine($"  Descrição: {resultado.Transacao.Descricao}");

            if (resultado.TipoCorrespondencia != TipoCorrespondencia.Nenhuma)
            {
                Console.WriteLine($"  ✓ Reconciliada com: {resultado.LancamentoCorrespondente?.Id}");
                Console.WriteLine($"  Tipo: {resultado.TipoCorrespondencia}");
                Console.WriteLine($"  Confiança: {resultado.NivelConfianca:P0}");
                if (resultado.Observacoes.Any())
                {
                    Console.WriteLine($"  Obs: {string.Join(", ", resultado.Observacoes)}");
                }
            }
            else
            {
                Console.WriteLine($"  ✗ Não reconciliada");
            }
            Console.WriteLine();
        }

        // Estatísticas
        var reconciliadas = resultados.Count(r => r.TipoCorrespondencia != TipoCorrespondencia.Nenhuma);
        Console.WriteLine($"📊 Estatísticas:");
        Console.WriteLine($"  Total de transações: {resultados.Count}");
        Console.WriteLine($"  Reconciliadas: {reconciliadas}");
        Console.WriteLine($"  Não reconciliadas: {resultados.Count - reconciliadas}");
        Console.WriteLine($"  Taxa de sucesso: {(reconciliadas * 100.0 / resultados.Count):F1}%");
    }

    static void ExemploGeracaoFicheirosPagamento()
    {
        Console.WriteLine("💳 Exemplo 2: Geração de Ficheiros de Pagamento\n");

        // Criar lista de pagamentos
        var pagamentos = new List<Pagamento>
        {
            new Pagamento
            {
                Id = "P001",
                DataPagamento = new DateTime(2026, 1, 10),
                Valor = 15000.00m,
                Beneficiario = "Fornecedor ABC Limitada",
                ContaBeneficiario = "000123456789",
                BancoBeneficiario = "BCI",
                Referencia = "INV-2026-001",
                Descricao = "Pagamento fatura materiais",
                Tipo = TipoPagamento.Transferencia
            },
            new Pagamento
            {
                Id = "P002",
                DataPagamento = new DateTime(2026, 1, 10),
                Valor = 8500.50m,
                Beneficiario = "Prestador Serviços XYZ",
                ContaBeneficiario = "000987654321",
                BancoBeneficiario = "Millennium BIM",
                Referencia = "SERV-456",
                Descricao = "Pagamento serviços consultoria",
                Tipo = TipoPagamento.Transferencia
            }
        };

        var caminhoTemp = Path.GetTempPath();

        // Gerar ficheiro CSV para Millennium BIM
        var geradorMBIM = new GeradorPagamentoMillenniumBIM();
        var ficheiroCsv = Path.Combine(caminhoTemp, "pagamentos_mbim.csv");
        geradorMBIM.GerarFicheiro(pagamentos, ficheiroCsv);
        Console.WriteLine($"✓ Ficheiro CSV gerado: {ficheiroCsv}");
        MostrarConteudoFicheiro(ficheiroCsv);

        // Gerar ficheiro XML para Standard Bank
        var geradorStdBank = new GeradorPagamentoStandardBank();
        var ficheiroXml = Path.Combine(caminhoTemp, "pagamentos_stdbank.xml");
        geradorStdBank.GerarFicheiro(pagamentos, ficheiroXml);
        Console.WriteLine($"\n✓ Ficheiro XML gerado: {ficheiroXml}");
        MostrarConteudoFicheiro(ficheiroXml, maxLinhas: 15);

        // Gerar ficheiro TXT para BCI
        var geradorBCI = new GeradorPagamentoBCI();
        var ficheiroTxt = Path.Combine(caminhoTemp, "pagamentos_bci.txt");
        geradorBCI.GerarFicheiro(pagamentos, ficheiroTxt);
        Console.WriteLine($"\n✓ Ficheiro TXT gerado: {ficheiroTxt}");
        MostrarConteudoFicheiro(ficheiroTxt);

        Console.WriteLine($"\n📁 Total de {pagamentos.Count} pagamentos processados");
        Console.WriteLine($"💰 Valor total: {pagamentos.Sum(p => p.Valor):C} MZN");
    }

    static void MostrarConteudoFicheiro(string caminho, int maxLinhas = 10)
    {
        Console.WriteLine("\nConteúdo:");
        Console.WriteLine(new string('-', 60));
        var linhas = File.ReadAllLines(caminho);
        var linhasParaMostrar = Math.Min(linhas.Length, maxLinhas);
        for (int i = 0; i < linhasParaMostrar; i++)
        {
            Console.WriteLine(linhas[i]);
        }
        if (linhas.Length > maxLinhas)
        {
            Console.WriteLine($"... ({linhas.Length - maxLinhas} linhas omitidas)");
        }
        Console.WriteLine(new string('-', 60));
    }
}

