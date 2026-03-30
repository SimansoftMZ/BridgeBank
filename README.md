# Simansoft.BridgeBank

[![.NET](https://img.shields.io/badge/.NET-10.0-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Simansoft.BridgeBank.Core)](https://www.nuget.org/packages/Simansoft.BridgeBank.Core)
[![Docker](https://img.shields.io/docker/v/simansoft/bridgebank-api?label=docker)](https://hub.docker.com/r/simansoft/bridgebank-api)

BridgeBank is an open-source .NET toolkit for bank reconciliation in ERP systems. It provides intelligent transaction matching, automatic classification (rule-based and ML), bank statement parsing, and payment file generation for Mozambican and international banks.

Available as **NuGet packages** for .NET, a **REST API** (Docker) for any language, and **typed SDKs** for Python, TypeScript and Java.

## Features

- **Bank Reconciliation** — intelligent matching engine with pluggable strategies (reference, value+date, description similarity, ML-based)
- **Transaction Classification** — rule-based and ML.NET-powered automatic categorisation (salaries, taxes, bank fees, suppliers, etc.)
- **Bank Statement Parsing** — reads Excel extracts from 6 Mozambican banks
- **Payment File Generation** — produces bank-specific payment files (TXT, CSV, XML)
- **REST API** — exposes all functionality over HTTP with OpenAPI spec and Scalar UI
- **Multi-language SDKs** — typed clients for Python, TypeScript and Java via Kiota

## Supported Banks

| Bank | Statement Parser | Payment Generator |
|------|:---:|:---:|
| BCI (Banco Comercial e de Investimentos) | Yes | Yes (TXT) |
| Millennium BIM | Yes | Yes (CSV) |
| Standard Bank | Yes | Yes (XML) |
| NedBank | Yes | — |
| Access Bank | Yes | — |
| M-Pesa | Yes | — |

## Installation

### NuGet Packages (.NET)

```bash
dotnet add package Simansoft.BridgeBank.Core
dotnet add package Simansoft.BridgeBank.Parsers
dotnet add package Simansoft.BridgeBank.Generators
```

For ML-based classification and reconciliation:

```bash
dotnet add package Simansoft.BridgeBank.ML
```

### Docker (any language)

```bash
docker pull simansoft/bridgebank-api
docker run -p 8080:8080 simansoft/bridgebank-api
```

Or with Docker Compose:

```bash
git clone https://github.com/SimansoftMZ/BridgeBank.git
cd BridgeBank
docker compose up --build
```

The API will be available at `http://localhost:8080` with the interactive Scalar UI at `http://localhost:8080/scalar/v1`.

### SDKs (Python, TypeScript, Java)

**Python:**

```bash
pip install -r sdks/python/requirements.txt
```

**TypeScript:**

```bash
cd sdks/typescript && npm install
```

**Java** — add to your `pom.xml`:

```xml
<dependency>
    <groupId>com.microsoft.kiota</groupId>
    <artifactId>microsoft-kiota-bundle</artifactId>
    <version>1.9.0</version>
</dependency>
```

### Build from Source

```bash
git clone https://github.com/SimansoftMZ/BridgeBank.git
cd BridgeBank
dotnet build
```

## Quick Start

### Reconciliation

```csharp
using Simansoft.BridgeBank.Core;
using Simansoft.BridgeBank.Core.Strategies;

var motor = new MotorReconciliacao();
motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorReferencia());
motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorValorEData());
motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorDescricao());

var resultados = motor.Reconciliar(transacoesBancarias, lancamentosERP);

foreach (var r in resultados.Where(r => r.TipoCorrespondencia != TipoCorrespondencia.Nenhuma))
{
    Console.WriteLine($"{r.Transacao.Id} -> {r.LancamentoCorrespondente?.Id} ({r.NivelConfianca:P0})");
}
```

### Transaction Classification

```csharp
using Simansoft.BridgeBank.Core.Classificacao;

using var classificador = ClassificadorTransacao.CriarComRegrasPadrao();
var resultado = classificador.Classificar(transacao);

Console.WriteLine($"Categoria: {resultado.Categoria}, Confianca: {resultado.Confianca:P0}");
```

### Bank Statement Parsing

```csharp
using Simansoft.BridgeBank.Parsers.Bancos;

var leitor = new LeitorExtratoBCI();
var extrato = leitor.LerExtrato("extrato_bci.xls");

Console.WriteLine($"Banco: {extrato.Banco}");
Console.WriteLine($"Conta: {extrato.NumeroConta}");
Console.WriteLine($"Transacoes: {extrato.Transacoes.Count}");
```

### Payment File Generation

```csharp
using Simansoft.BridgeBank.Generators.Bancos;
using Simansoft.BridgeBank.Generators.Models;

var pagamentos = new List<Pagamento>
{
    new()
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

var gerador = new GeradorPagamentoBCI();
gerador.GerarFicheiro(pagamentos, "pagamentos.txt");
```

### ML Classification

```csharp
using Simansoft.BridgeBank.ML.Classificacao;

// Train a model
var treinador = new TreinadorClassificacao();
var metricas = treinador.TreinarComDadosSinteticos("modelo.zip");

// Use it in the classifier
using var classificador = ClassificadorTransacao.CriarComRegrasPadrao();
classificador.RegistrarRegra(new RegraClassificacaoML("modelo.zip", prioridade: 40));
```

### REST API (from any language)

```bash
# Classify transactions
curl -X POST http://localhost:8080/api/classificacao \
  -H "Content-Type: application/json" \
  -d '{"transacoes": [{"id": "1", "data": "2026-01-15", "valor": 1500, "descricao": "PAGAMENTO SALARIO", "tipo": "Debito", "categoria": "NaoClassificada", "confiancaClassificacao": 0}]}'

# Parse a bank statement (file upload)
curl -X POST http://localhost:8080/api/extratos/parse \
  -F "file=@extrato_bci.xls"

# List supported banks
curl http://localhost:8080/api/bancos/parsers
```

## REST API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/health` | Health check |
| `GET` | `/api/bancos/parsers` | List banks with statement parsers |
| `GET` | `/api/bancos/geradores` | List banks with payment generators |
| `POST` | `/api/extratos/parse` | Upload and parse a bank statement file |
| `POST` | `/api/classificacao` | Classify transactions (JSON) |
| `POST` | `/api/classificacao/extrato` | Upload a statement and classify all transactions |
| `POST` | `/api/reconciliacao` | Reconcile transactions with ERP entries |
| `POST` | `/api/pagamentos/gerar` | Generate and download a payment file |
| `GET` | `/api/enums/categorias` | List transaction categories |
| `GET` | `/api/enums/tipos-transacao` | List transaction types |
| `GET` | `/api/enums/tipos-correspondencia` | List match types |

Interactive documentation: `http://localhost:8080/scalar/v1`
OpenAPI spec: `http://localhost:8080/openapi/v1.json`

## Project Structure

```
BridgeBank/
├── src/
│   ├── BridgeBank.Core/           # Reconciliation engine, classification, models
│   │   ├── Models/                # Domain models (Transacao, ExtratoBancario, etc.)
│   │   ├── Interfaces/            # Contracts (IMotorReconciliacao, IRegraClassificacao, etc.)
│   │   ├── Strategies/            # Matching strategies (reference, value+date, description)
│   │   ├── Classificacao/         # Rule-based transaction classifier
│   │   └── MotorReconciliacao.cs  # Main reconciliation engine
│   ├── BridgeBank.Parsers/        # Bank statement readers
│   │   ├── Interfaces/            # ILeitorExtrato contract
│   │   ├── Excel/                 # Base Excel reader (NPOI)
│   │   └── Bancos/                # Bank-specific implementations
│   ├── BridgeBank.Generators/     # Payment file generators
│   │   ├── Interfaces/            # IGeradorFicheiroPagamento contract
│   │   ├── Models/                # Pagamento model
│   │   └── Bancos/                # Bank-specific implementations (TXT, CSV, XML)
│   ├── BridgeBank.ML/             # Machine learning classification & reconciliation
│   │   ├── Classificacao/         # ML.NET transaction classifier
│   │   ├── Reconciliacao/         # ML.NET matching strategy
│   │   ├── Dados/                 # Synthetic training data generator
│   │   └── Infraestrutura/        # Model management and configuration
│   └── BridgeBank.Api/            # REST API (ASP.NET Core Minimal API)
│       ├── Dtos/                  # Request/response DTOs
│       ├── Mappers/               # Model <-> DTO mapping
│       ├── Dockerfile             # Multi-stage Docker build
│       └── Program.cs             # API endpoints and service registration
├── tests/                         # Unit tests for each project
├── samples/                       # Usage examples
├── sdks/                          # Generated API clients (Kiota)
│   ├── python/                    # Python SDK
│   ├── typescript/                # TypeScript SDK
│   └── java/                      # Java SDK
├── openapi/                       # Auto-generated OpenAPI spec
├── scripts/                       # SDK regeneration scripts
├── docker-compose.yml             # Docker Compose for the API
└── dotnet-tools.json              # Local tool manifest (Kiota)
```

## Extensibility

### Custom Matching Strategy

```csharp
public class MinhaEstrategia : IEstrategiaCorrespondencia
{
    public int Prioridade => 50;

    public ResultadoCorrespondencia? TentarCorrespondencia(
        Transacao transacao,
        IEnumerable<LancamentoERP> lancamentos)
    {
        // Custom matching logic
        return null;
    }
}

motor.RegistrarEstrategia(new MinhaEstrategia());
```

### Custom Classification Rule

```csharp
public class MinhaRegra : IRegraClassificacao
{
    public int Prioridade => 60;
    public string Nome => "MinhaRegra";

    public ResultadoClassificacao? Classificar(Transacao transacao)
    {
        // Custom classification logic
        return null;
    }
}

classificador.RegistrarRegra(new MinhaRegra());
```

### Adding a New Bank Parser

Extend `LeitorExcelBase` and implement `ILeitorExtrato`:

```csharp
public class LeitorExtratoNovoBanco : LeitorExcelBase, ILeitorExtrato
{
    public bool SuportaArquivo(string caminhoArquivo) =>
        caminhoArquivo.EndsWith(".xls", StringComparison.OrdinalIgnoreCase);

    public ExtratoBancario LerExtrato(string caminhoArquivo)
    {
        var workbook = AbrirFicheiro(caminhoArquivo);
        // Bank-specific parsing logic
    }
}
```

## Regenerating SDKs

After modifying the API, regenerate the typed SDKs:

```bash
# Linux/macOS
./scripts/generate-sdks.sh

# Windows
pwsh scripts/generate-sdks.ps1
```

This rebuilds the API, regenerates the OpenAPI spec, and updates all three SDKs (Python, TypeScript, Java) via Kiota.

## Tests

```bash
# Run all tests
dotnet test

# Run tests for a specific project
dotnet test tests/BridgeBank.Core.Tests

# With coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Code Conventions

- **Code in Portuguese**: classes, methods, variables and comments in Portuguese
- **Structure in English**: project names, folders and namespaces in English
- **XML documentation**: all public classes and methods have XML comments

## Contributing

Contributions are welcome! Please:

1. Fork the project
2. Create a branch for your feature (`git checkout -b feature/MyFeature`)
3. Commit your changes (`git commit -m 'Add MyFeature'`)
4. Push to the branch (`git push origin feature/MyFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Authors

- **Simansoft** - *Initial development*

## Support

For questions and support, please open an [issue](https://github.com/SimansoftMZ/BridgeBank/issues) on GitHub.
