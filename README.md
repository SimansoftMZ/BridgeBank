# Simansoft.BridgeBank

[![.NET](https://img.shields.io/badge/.NET-10.0-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Simansoft.BridgeBank.Core)](https://www.nuget.org/packages/Simansoft.BridgeBank.Core)
[![Docker](https://img.shields.io/docker/v/simansoft/bridgebank-api?label=docker)](https://hub.docker.com/r/simansoftmz/bridgebank-api)

O BridgeBank é um toolkit .NET de código aberto para reconciliação bancária em sistemas ERP. Oferece correspondência inteligente de transacções, classificação automática (baseada em regras e ML), leitura de extractos bancários e geração de ficheiros de pagamento para bancos moçambicanos e internacionais.

Disponível como **pacotes NuGet** para .NET, uma **API REST** (Docker) para qualquer linguagem, e **SDKs tipados** para Python, TypeScript e Java.

## Funcionalidades

- **Reconciliação Bancária** — motor de correspondência inteligente com estratégias configuráveis (referência, valor+data, similaridade de descrição, baseado em ML)
- **Classificação de Transacções** — categorização automática baseada em regras e ML.NET (salários, impostos, taxas bancárias, fornecedores, etc.)
- **Leitura de Extractos Bancários** — lê extractos Excel de 6 bancos moçambicanos
- **Geração de Ficheiros de Pagamento** — produz ficheiros de pagamento específicos por banco (TXT, CSV, XML)
- **API REST** — expõe todas as funcionalidades via HTTP com especificação OpenAPI e interface Scalar
- **SDKs Multi-linguagem** — clientes tipados para Python, TypeScript e Java via Kiota

## Bancos Suportados

| Banco | Leitor de Extracto | Gerador de Pagamento |
|-------|:---:|:---:|
| BCI (Banco Comercial e de Investimentos) | Sim | Sim (TXT) |
| Millennium BIM | Sim | Sim (CSV) |
| Standard Bank | Sim | Sim (XML) |
| NedBank | Sim | — |
| Access Bank | Sim | — |
| M-Pesa | Sim | — |

## Instalação

### Pacotes NuGet (.NET)

```bash
dotnet add package Simansoft.BridgeBank.Core
dotnet add package Simansoft.BridgeBank.Parsers
dotnet add package Simansoft.BridgeBank.Generators
```

Para classificação e reconciliação baseadas em ML:

```bash
dotnet add package Simansoft.BridgeBank.ML
```

### Docker (qualquer linguagem)

```bash
docker pull simansoft/bridgebank-api
docker run -p 8080:8080 simansoft/bridgebank-api
```

Ou com Docker Compose:

```bash
git clone https://github.com/SimansoftMZ/BridgeBank.git
cd BridgeBank
docker compose up --build
```

A API ficará disponível em `http://localhost:8080` com a interface interativa Scalar em `http://localhost:8080/scalar/v1`.

### SDKs (Python, TypeScript, Java)

**Python:**

```bash
pip install -r sdks/python/requirements.txt
```

**TypeScript:**

```bash
cd sdks/typescript && npm install
```

**Java** — adicione ao seu `pom.xml`:

```xml
<dependency>
    <groupId>com.microsoft.kiota</groupId>
    <artifactId>microsoft-kiota-bundle</artifactId>
    <version>1.9.0</version>
</dependency>
```

### Compilar a partir do Código Fonte

```bash
git clone https://github.com/SimansoftMZ/BridgeBank.git
cd BridgeBank
dotnet build
```

## Início Rápido

### Reconciliação

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

### Classificação de Transacções

```csharp
using Simansoft.BridgeBank.Core.Classificacao;

using var classificador = ClassificadorTransacao.CriarComRegrasPadrao();
var resultado = classificador.Classificar(transacao);

Console.WriteLine($"Categoria: {resultado.Categoria}, Confianca: {resultado.Confianca:P0}");
```

### Leitura de Extractos Bancários

```csharp
using Simansoft.BridgeBank.Parsers.Bancos;

var leitor = new LeitorExtratoBCI();
var extrato = leitor.LerExtrato("extrato_bci.xls");

Console.WriteLine($"Banco: {extrato.Banco}");
Console.WriteLine($"Conta: {extrato.NumeroConta}");
Console.WriteLine($"Transacoes: {extrato.Transacoes.Count}");
```

### Geração de Ficheiros de Pagamento

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

### Classificação com ML

```csharp
using Simansoft.BridgeBank.ML.Classificacao;

// Treinar um modelo
var treinador = new TreinadorClassificacao();
var metricas = treinador.TreinarComDadosSinteticos("modelo.zip");

// Usar o modelo no classificador
using var classificador = ClassificadorTransacao.CriarComRegrasPadrao();
classificador.RegistrarRegra(new RegraClassificacaoML("modelo.zip", prioridade: 40));
```

### API REST (a partir de qualquer linguagem)

```bash
# Classificar transacções
curl -X POST http://localhost:8080/api/classificacao \
  -H "Content-Type: application/json" \
  -d '{"transacoes": [{"id": "1", "data": "2026-01-15", "valor": 1500, "descricao": "PAGAMENTO SALARIO", "tipo": "Debito", "categoria": "NaoClassificada", "confiancaClassificacao": 0}]}'

# Ler um extracto bancário (upload de ficheiro)
curl -X POST http://localhost:8080/api/extratos/parse \
  -F "file=@extrato_bci.xls"

# Listar bancos suportados
curl http://localhost:8080/api/bancos/parsers
```

## Endpoints da API REST

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/health` | Verificação de estado |
| `GET` | `/api/bancos/parsers` | Listar bancos com leitores de extracto |
| `GET` | `/api/bancos/geradores` | Listar bancos com geradores de pagamento |
| `POST` | `/api/extratos/parse` | Carregar e ler um ficheiro de extracto bancário |
| `POST` | `/api/classificacao` | Classificar transacções (JSON) |
| `POST` | `/api/classificacao/extrato` | Carregar um extracto e classificar todas as transacções |
| `POST` | `/api/reconciliacao` | Reconciliar transacções com lançamentos ERP |
| `POST` | `/api/pagamentos/gerar` | Gerar e descarregar um ficheiro de pagamento |
| `GET` | `/api/enums/categorias` | Listar categorias de transacções |
| `GET` | `/api/enums/tipos-transacao` | Listar tipos de transacção |
| `GET` | `/api/enums/tipos-correspondencia` | Listar tipos de correspondência |

Documentação interativa: `http://localhost:8080/scalar/v1`
Especificação OpenAPI: `http://localhost:8080/openapi/v1.json`

## Estrutura do Projecto

```
BridgeBank/
├── src/
│   ├── BridgeBank.Core/           # Motor de reconciliação, classificação e modelos
│   │   ├── Models/                # Modelos de domínio (Transacao, ExtratoBancario, etc.)
│   │   ├── Interfaces/            # Contratos (IMotorReconciliacao, IRegraClassificacao, etc.)
│   │   ├── Strategies/            # Estratégias de correspondência (referência, valor+data, descrição)
│   │   ├── Classificacao/         # Classificador de transacções baseado em regras
│   │   └── MotorReconciliacao.cs  # Motor principal de reconciliação
│   ├── BridgeBank.Parsers/        # Leitores de extractos bancários
│   │   ├── Interfaces/            # Contrato ILeitorExtrato
│   │   ├── Excel/                 # Leitor base Excel (NPOI)
│   │   └── Bancos/                # Implementações específicas por banco
│   ├── BridgeBank.Generators/     # Geradores de ficheiros de pagamento
│   │   ├── Interfaces/            # Contrato IGeradorFicheiroPagamento
│   │   ├── Models/                # Modelo Pagamento
│   │   └── Bancos/                # Implementações específicas por banco (TXT, CSV, XML)
│   ├── BridgeBank.ML/             # Classificação e reconciliação com aprendizagem automática
│   │   ├── Classificacao/         # Classificador de transacções ML.NET
│   │   ├── Reconciliacao/         # Estratégia de correspondência ML.NET
│   │   ├── Dados/                 # Gerador de dados de treino sintéticos
│   │   └── Infraestrutura/        # Gestão de modelos e configuração
│   └── BridgeBank.Api/            # API REST (ASP.NET Core Minimal API)
│       ├── Dtos/                  # DTOs de pedido/resposta
│       ├── Mappers/               # Mapeamento Modelo <-> DTO
│       ├── Dockerfile             # Build Docker multi-etapa
│       └── Program.cs             # Endpoints da API e registo de serviços
├── tests/                         # Testes unitários para cada projecto
├── samples/                       # Exemplos de utilização
├── sdks/                          # Clientes da API gerados (Kiota)
│   ├── python/                    # SDK Python
│   ├── typescript/                # SDK TypeScript
│   └── java/                      # SDK Java
├── openapi/                       # Especificação OpenAPI gerada automaticamente
├── scripts/                       # Scripts de regeneração dos SDKs
├── docker-compose.yml             # Docker Compose para a API
└── dotnet-tools.json              # Manifesto de ferramentas locais (Kiota)
```

## Extensibilidade

### Estratégia de Correspondência Personalizada

```csharp
public class MinhaEstrategia : IEstrategiaCorrespondencia
{
    public int Prioridade => 50;

    public ResultadoCorrespondencia? TentarCorrespondencia(
        Transacao transacao,
        IEnumerable<LancamentoERP> lancamentos)
    {
        // Lógica de correspondência personalizada
        return null;
    }
}

motor.RegistrarEstrategia(new MinhaEstrategia());
```

### Regra de Classificação Personalizada

```csharp
public class MinhaRegra : IRegraClassificacao
{
    public int Prioridade => 60;
    public string Nome => "MinhaRegra";

    public ResultadoClassificacao? Classificar(Transacao transacao)
    {
        // Lógica de classificação personalizada
        return null;
    }
}

classificador.RegistrarRegra(new MinhaRegra());
```

### Adicionar um Novo Leitor de Banco

Extenda `LeitorExcelBase` e implemente `ILeitorExtrato`:

```csharp
public class LeitorExtratoNovoBanco : LeitorExcelBase, ILeitorExtrato
{
    public bool SuportaArquivo(string caminhoArquivo) =>
        caminhoArquivo.EndsWith(".xls", StringComparison.OrdinalIgnoreCase);

    public ExtratoBancario LerExtrato(string caminhoArquivo)
    {
        var workbook = AbrirFicheiro(caminhoArquivo);
        // Lógica de leitura específica do banco
    }
}
```

## Regenerar os SDKs

Após modificar a API, regenere os SDKs tipados:

```bash
# Linux/macOS
./scripts/generate-sdks.sh

# Windows
pwsh scripts/generate-sdks.ps1
```

Este processo reconstrói a API, regenera a especificação OpenAPI e actualiza os três SDKs (Python, TypeScript, Java) via Kiota.

## Testes

```bash
# Executar todos os testes
dotnet test

# Executar testes de um projecto específico
dotnet test tests/BridgeBank.Core.Tests

# Com cobertura de código
dotnet test --collect:"XPlat Code Coverage"
```

## Contribuição

Contribuições são bem-vindas! Consulte o [CONTRIBUTING.md](CONTRIBUTING.md) para convenções de código, estratégia de branches, CI/CD e instruções detalhadas sobre como submeter Pull Requests.

## Licença

Este projecto está licenciado sob a Licença MIT — consulte o ficheiro [LICENSE](LICENSE) para mais detalhes.

## Autores

- **Simansoft** - *Desenvolvimento inicial*

## Suporte

Para questões e suporte, por favor abra uma [issue](https://github.com/SimansoftMZ/BridgeBank/issues) no GitHub.
