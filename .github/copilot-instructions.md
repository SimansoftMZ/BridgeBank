# BridgeBank - AI Coding Agent Instructions

## Project Overview

BridgeBank is a .NET 10.0 library for bank reconciliation in ERP systems, targeting Mozambican banks. The codebase is written in **Portuguese** (code identifiers, comments, documentation) but uses English for project/namespace structure.

**Core Architecture (3 main components):**

1. **BridgeBank.Core** - Reconciliation engine using Strategy pattern
2. **BridgeBank.Parsers** - Bank statement readers (Excel/PDF) with bank-specific implementations
3. **BridgeBank.Generators** - Payment file generators (CSV/XML/TXT) per bank format

## Critical Conventions

### Naming & Language

- **Classes, methods, properties, variables**: Portuguese with PascalCase/camelCase
  - `MotorReconciliacao`, `ReconciliarTransacoes()`, `var transacaoBancaria`
- **Namespaces, project names**: English with PascalCase
  - `BridgeBank.Core.Models`, `BridgeBank.Parsers.Bancos`
- **XML documentation**: Always in Portuguese for public APIs
  ```csharp
  /// <summary>
  /// Reconcilia transações bancárias com lançamentos ERP
  /// </summary>
  ```

### Testing Patterns (xUnit)

- Test method naming: `[Method]_[Scenario]_[ExpectedResult]` in Portuguese
  - `Reconciliar_ComReferenciaExata_DeveRetornarCorrespondencia()`
- Use AAA pattern (Arrange, Act, Assert) with clear comments
- Test projects mirror source structure: `BridgeBank.Core.Tests`

## Key Design Patterns

### Strategy Pattern for Reconciliation

The reconciliation engine (`MotorReconciliacao`) uses **priority-based strategy execution**:

```csharp
// Strategies execute in priority order (higher = first)
// Each strategy gets a chance until one matches
var motor = new MotorReconciliacao();
motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorReferencia()); // Priority 100
motor.RegistrarEstrategia(new EstrategiaCorrespondenciaPorValorEData()); // Priority 75
```

**When adding new strategies:**
- Implement `IEstrategiaCorrespondencia`
- Set `Prioridade` appropriately (100=exact match, 50=fuzzy, 25=low confidence)
- Return `null` if no match found
- Remove matched `LancamentoERP` from available pool (handled by motor)

### Template Method for Bank Parsers

Base class `LeitorExcelBase` defines the structure; bank-specific classes override column mappings:

```csharp
// Each bank parser defines its Excel layout
private const int LinhaInicioTransacoes = 10;
private const int ColunaData = 1;
private const int ColunaDescricao = 2;
```

**When adding new banks:**
- Inherit from `LeitorExcelBase` (Excel) or create new base for PDF
- Override abstract methods: `ObterData()`, `ObterValor()`, `ObterDescricao()`
- Define column constants at class level
- See [LeitorExtratoBCI.cs](../src/BridgeBank.Parsers/Bancos/LeitorExtratoBCI.cs) as reference

### Bank-Specific Generators

Each bank has unique file format requirements (CSV, XML, TXT with specific field orders):

- `GeradorPagamentoBCI` → TXT with fixed-width fields
- `GeradorPagamentoMillenniumBIM` → CSV
- `GeradorPagamentoStandardBank` → XML

Check existing implementations before creating new ones - banks often share format families.

## Developer Workflows

### Building & Running

```bash
# Build entire solution
dotnet build

# Run all tests
dotnet test

# Run sample application
cd samples/BridgeBank.Samples
dotnet run
```

### EPPlus License Configuration

**IMPORTANT**: EPPlus requires license context initialization before use. Already configured in `ConfiguracaoEPPlus.cs`:

```csharp
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
```

Don't create new configurations - this is centralized. Tests use `InternalsVisibleTo` for access.

### Project Dependencies

- **BridgeBank.Core** → No dependencies (pure business logic)
- **BridgeBank.Parsers** → References Core + EPPlus (Excel) + iTextSharp (PDF)
- **BridgeBank.Generators** → References Core only

**Never add dependencies to Core** - keep it portable and framework-agnostic.

## Common Tasks

### Adding a New Matching Strategy

1. Create class in `src/BridgeBank.Core/Strategies/`
2. Implement `IEstrategiaCorrespondencia`
3. Set appropriate `Prioridade` (consult existing strategies)
4. Add tests in `tests/BridgeBank.Core.Tests/`
5. Update sample in `Program.cs` to demonstrate usage

### Supporting a New Bank

**For Excel parsers:**
1. Add class `LeitorExtrato[BankName].cs` in `Bancos/`
2. Extend `LeitorExcelBase`
3. Define column constants
4. Override extraction methods
5. Test with real bank statement Excel file

**For payment generators:**
1. Review bank's file format specification
2. Create `GeradorPagamento[BankName].cs` in `Generators/Bancos/`
3. Implement `IGeradorFicheiroPagamento`
4. Add validation for bank-specific rules
5. Create tests with sample payment data

## Model Structure

Key domain models in `BridgeBank.Core.Models`:

- `Transacao` - Bank transaction (from statements)
- `LancamentoERP` - ERP entry (to match against)
- `ExtratoBancario` - Complete statement with metadata
- `ResultadoReconciliacao` - Match result with confidence level
- `Pagamento` - Payment instruction for file generation

All models use nullable reference types (`Nullable` enabled) - respect null safety.

## Testing Expectations

- **Unit tests required** for all new strategies and bank implementations
- Use `[Fact]` for single test cases, `[Theory]` for parameterized tests
- Mock external dependencies (files should be test fixtures, not live data)
- Place test data files in `TestData/` subdirectory within test project
- All tests should be independent and runnable in any order

## Documentation Standards

- Every public method/class needs XML documentation in Portuguese
- Update README.md if adding new bank support or major features
- Keep code examples in README synchronized with actual `samples/` code
- Update ROADMAP.md when completing planned features

## Quick Reference

- Solution file: `BridgeBank.sln`
- Main entry point: `src/BridgeBank.Core/MotorReconciliacao.cs`
- Example usage: `samples/BridgeBank.Samples/Program.cs`
- Target framework: .NET 10.0
- Test framework: xUnit