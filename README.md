# BridgeBank

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

BridgeBank é uma biblioteca .NET open-source que resolve o problema de reconciliação bancária em sistemas ERP. Oferece correspondência inteligente de transações, classificação automática de taxas e comissões, e geração de ficheiros de pagamento em formatos específicos de cada banco. Suporta bancos moçambicanos e formatos internacionais.

## 🌟 Características

- **Reconciliação Automatizada**: Motor de reconciliação inteligente com múltiplas estratégias
- **Estratégias de Correspondência**:
  - Correspondência por referência exata
  - Correspondência por valor e data (com tolerâncias configuráveis)
  - Correspondência por similaridade de descrição
- **Leitura de Extractos Bancários**:
  - Suporte para ficheiros Excel (.xlsx, .xls)
  - Suporte para ficheiros PDF (em desenvolvimento)
- **Geração de Ficheiros de Pagamento**:
  - Formatos específicos para cada banco
  - Validação automática de dados
- **Bancos Suportados**:
  - BCI (Banco Comercial e de Investimentos)
  - Millennium BIM
  - Standard Bank
  - Arquitetura extensível para outros bancos

## 📦 Instalação

### Via NuGet (quando publicado)

```bash
dotnet add package BridgeBank.Core
dotnet add package BridgeBank.Parsers
dotnet add package BridgeBank.Generators
```

### Compilar do Código Fonte

```bash
git clone https://github.com/SimansoftMZ/BridgeBank.git
cd BridgeBank
dotnet build
```

## 🚀 Uso Rápido

### Reconciliação Básica

```csharp
using BridgeBank.Core;
using BridgeBank.Core.Strategies;

// Criar motor de reconciliação
var motor = new MotorReconciliacao();

// Registrar estratégias
motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorReferencia());
motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorValorEData());
motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorDescricao());

// Reconciliar transações
var resultados = motor.Reconciliar(transacoesBancarias, lancamentosERP);

// Processar resultados
foreach (var resultado in resultados)
{
    if (resultado.TipoCorrespondencia != TipoCorrespondencia.Nenhuma)
    {
        Console.WriteLine($"Transação {resultado.Transacao.Id} reconciliada com " +
                         $"lançamento {resultado.LancamentoCorrespondente?.Id}");
        Console.WriteLine($"Confiança: {resultado.NivelConfianca:P0}");
    }
}
```

### Leitura de Extracto Bancário

```csharp
using BridgeBank.Parsers.Bancos;

// Ler extrato do BCI
var leitor = new LeitorExtratoBCI();
var extrato = leitor.LerExtrato("caminho/para/extrato_bci.xlsx");

Console.WriteLine($"Banco: {extrato.Banco}");
Console.WriteLine($"Conta: {extrato.NumeroConta}");
Console.WriteLine($"Período: {extrato.DataInicio:d} a {extrato.DataFim:d}");
Console.WriteLine($"Transações: {extrato.Transacoes.Count}");
```

### Geração de Ficheiro de Pagamento

```csharp
using BridgeBank.Generators.Bancos;
using BridgeBank.Generators.Models;

// Criar lista de pagamentos
var pagamentos = new List<Pagamento>
{
    new Pagamento
    {
        Id = "P001",
        DataPagamento = DateTime.Now,
        Valor = 1000.00m,
        Beneficiario = "Fornecedor XYZ",
        ContaBeneficiario = "123456789",
        BancoBeneficiario = "BCI",
        Referencia = "INV-001"
    }
};

// Gerar ficheiro para Millennium BIM (CSV)
var gerador = new GeradorPagamentoMillenniumBIM();
gerador.GerarFicheiro(pagamentos, "pagamentos_mbim.csv");

// Gerar ficheiro para Standard Bank (XML)
var geradorStdBank = new GeradorPagamentoStandardBank();
geradorStdBank.GerarFicheiro(pagamentos, "pagamentos_stdbank.xml");
```

## 🏗️ Arquitetura

```
BridgeBank/
├── src/
│   ├── BridgeBank.Core/           # Motor de reconciliação e estratégias
│   │   ├── Models/                # Modelos de domínio
│   │   ├── Interfaces/            # Contratos
│   │   ├── Strategies/            # Estratégias de correspondência
│   │   └── MotorReconciliacao.cs  # Motor principal
│   ├── BridgeBank.Parsers/        # Leitores de extractos
│   │   ├── Interfaces/            # Contratos de leitores
│   │   ├── Excel/                 # Leitores Excel base
│   │   ├── Pdf/                   # Leitores PDF
│   │   └── Bancos/                # Implementações específicas
│   └── BridgeBank.Generators/     # Geradores de ficheiros
│       ├── Interfaces/            # Contratos de geradores
│       ├── Models/                # Modelos de pagamento
│       └── Bancos/                # Implementações específicas
└── tests/                         # Testes unitários
```

## 🔧 Extensibilidade

### Criar Estratégia Customizada

```csharp
public class MinhaEstrategia : IEstrategiaCorrespondencia
{
    public int Prioridade => 50;

    public ResultadoCorrespondencia? TentarCorrespondencia(
        Transacao transacao,
        IEnumerable<LancamentoERP> lancamentos)
    {
        // Implementar lógica customizada
        return null;
    }
}
```

### Adicionar Suporte para Novo Banco

```csharp
public class LeitorExtratoNovoBanco : LeitorExcelBase
{
    public override ExtratoBancario LerExtrato(string caminhoArquivo)
    {
        // Implementar leitura específica do banco
    }

    // Implementar métodos abstratos
}
```

## 🧪 Testes

```bash
# Executar todos os testes
dotnet test

# Executar testes de um projeto específico
dotnet test tests/BridgeBank.Core.Tests

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## 📝 Convenções de Código

- **Código em Português**: Classes, métodos, variáveis e comentários em português
- **Estrutura em Inglês**: Nomes de projetos, pastas e namespaces em inglês
- **Documentação XML**: Todas as classes e métodos públicos devem ter comentários XML

## 🤝 Contribuir

Contribuições são bem-vindas! Por favor:

1. Faça fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adicionar MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está licenciado sob a MIT License - veja o arquivo [LICENSE](LICENSE) para detalhes.

## 👥 Autores

- **Simansoft MZ** - *Desenvolvimento inicial*

## 🙏 Agradecimentos

- Bancos de Moçambique por inspirarem esta solução
- Comunidade .NET por ferramentas excelentes
- EPPlus por facilitar leitura de Excel

## 📞 Suporte

Para questões e suporte, por favor abra uma [issue](https://github.com/SimansoftMZ/BridgeBank/issues) no GitHub.

