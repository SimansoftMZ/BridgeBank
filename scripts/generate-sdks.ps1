$ErrorActionPreference = "Stop"

$RootDir = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
$OpenApiSpec = Join-Path $RootDir "openapi\BridgeBank.Api.json"

Write-Host "=== BridgeBank SDK Generator ==="

# Step 1: Build API to regenerate OpenAPI spec
Write-Host "[1/4] Building API project to generate OpenAPI spec..."
dotnet build "$RootDir\src\BridgeBank.Api\BridgeBank.Api.csproj" --no-incremental -q
if (-not (Test-Path $OpenApiSpec)) {
    Write-Error "OpenAPI spec not found at $OpenApiSpec"
    exit 1
}
Write-Host "      OpenAPI spec generated at: $OpenApiSpec"

# Step 2: Restore Kiota tool
Write-Host "[2/4] Restoring Kiota tool..."
dotnet tool restore --tool-manifest "$RootDir\dotnet-tools.json" -q

# Step 3: Generate SDKs
Write-Host "[3/4] Generating SDKs..."

Write-Host "      -> Python..."
dotnet kiota generate `
    --language python `
    --openapi $OpenApiSpec `
    --output "$RootDir\sdks\python\bridgebank" `
    --class-name BridgeBankClient `
    --namespace-name bridgebank `
    --clean-output 2>$null

Write-Host "      -> TypeScript..."
dotnet kiota generate `
    --language typescript `
    --openapi $OpenApiSpec `
    --output "$RootDir\sdks\typescript\src" `
    --class-name BridgeBankClient `
    --namespace-name bridgebank `
    --clean-output 2>$null

Write-Host "      -> Java..."
dotnet kiota generate `
    --language java `
    --openapi $OpenApiSpec `
    --output "$RootDir\sdks\java\src\main\java\bridgebank" `
    --class-name BridgeBankClient `
    --namespace-name bridgebank `
    --clean-output 2>$null

# Step 4: Summary
Write-Host "[4/4] Done!"
Write-Host ""
Write-Host "Generated SDKs:"
Write-Host "  Python:     sdks\python\bridgebank\"
Write-Host "  TypeScript: sdks\typescript\src\"
Write-Host "  Java:       sdks\java\src\main\java\bridgebank\"
Write-Host ""
Write-Host "Install dependencies:"
Write-Host "  Python:     pip install -r sdks\python\requirements.txt"
Write-Host "  TypeScript: cd sdks\typescript && npm install"
Write-Host "  Java:       cd sdks\java && mvn install"
