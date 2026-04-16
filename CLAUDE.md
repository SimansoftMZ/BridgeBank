# CLAUDE.md

Este ficheiro fornece orientação ao Claude Code (claude.ai/code) quando trabalha com o código neste repositório.

## Sobre o Projecto

Toolkit .NET 10 open-source para reconciliação bancária em sistemas ERP moçambicanos. Publicado como pacotes NuGet (`Simansoft.BridgeBank.*`) e imagem Docker. Expõe também uma API REST com SDKs gerados para Python, TypeScript e Java.

## Comandos

```bash
# Compilar
dotnet build

# Testes
dotnet test
dotnet test tests/BridgeBank.Core.Tests        # projecto específico
dotnet test --collect:"XPlat Code Coverage"    # com cobertura

# Executar a API
dotnet run --project src/BridgeBank.Api
# http://localhost:8080 — API
# http://localhost:8080/scalar/v1 — documentação interactiva
# http://localhost:8080/openapi/v1.json — especificação OpenAPI

# Docker
docker compose up --build

# Regenerar SDKs (após alterar a API)
pwsh scripts/generate-sdks.ps1     # Windows
./scripts/generate-sdks.sh         # Linux/macOS
```

## Arquitectura

```
src/
├── BridgeBank.Core/       # Motor de reconciliação, classificador, modelos de domínio
├── BridgeBank.Parsers/    # Leitores de extractos bancários (Excel via NPOI)
├── BridgeBank.Generators/ # Geradores de ficheiros de pagamento (TXT, CSV, XML)
├── BridgeBank.ML/         # Classificação e reconciliação com ML.NET
└── BridgeBank.Api/        # Minimal API ASP.NET Core (Scalar + OpenAPI)
sdks/                      # Clientes tipados gerados por Kiota (Python, TypeScript, Java)
openapi/                   # Especificação OpenAPI gerada automaticamente no build
```

- **`BridgeBank.Core`** — Núcleo sem dependências externas, compatível com AOT. Contém `MotorReconciliacao`, `ClassificadorTransacao` e as interfaces de extensibilidade.
- **`BridgeBank.Parsers`** — Cada banco tem a sua implementação em `Bancos/`, estendendo `LeitorExcelBase`.
- **`BridgeBank.Generators`** — Cada banco tem a sua implementação em `Bancos/`, implementando `IGeradorFicheiroPagamento`.
- **`BridgeBank.ML`** — Liga-se ao Core via `IRegraClassificacao`. Pode treinar com dados sintéticos.
- **`BridgeBank.Api`** — Gera a especificação OpenAPI em `openapi/` durante o build. Os SDKs são gerados a partir daí.

### Pontos de Extensibilidade
- Nova estratégia de correspondência → implementar `IEstrategiaCorrespondencia`
- Nova regra de classificação → implementar `IRegraClassificacao`
- Novo banco (leitor) → estender `LeitorExcelBase` e implementar `ILeitorExtrato`
- Novo banco (gerador) → implementar `IGeradorFicheiroPagamento`

## Convenções de Código

- **Classes e métodos**: PascalCase em **português** (ex: `MotorReconciliacao`, `ReconciliarTransacoes`)
- **Variáveis locais**: camelCase em **português** (ex: `transacaoBancaria`)
- **Projectos e namespaces**: PascalCase em **inglês** (ex: `Simansoft.BridgeBank.Core`)
- Nullable reference types e `ImplicitUsings` activos

## Estratégia de Branches

GitHub Flow — todo o trabalho entra em `main` via Pull Request:

```
main (protegido)
  ├── feature/*
  ├── fix/*
  ├── docs/*
  └── chore/*
```

Tags `v*.*.*` em `main` disparam publicação automática no NuGet e Docker Hub.
