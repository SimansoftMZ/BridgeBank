#!/bin/bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
OPENAPI_SPEC="$ROOT_DIR/openapi/BridgeBank.Api.json"

echo "=== BridgeBank SDK Generator ==="

# Step 1: Build API to regenerate OpenAPI spec
echo "[1/4] Building API project to generate OpenAPI spec..."
dotnet build "$ROOT_DIR/src/BridgeBank.Api/BridgeBank.Api.csproj" --no-incremental -q

if [ ! -f "$OPENAPI_SPEC" ]; then
    echo "ERROR: OpenAPI spec not found at $OPENAPI_SPEC"
    exit 1
fi
echo "      OpenAPI spec generated at: $OPENAPI_SPEC"

# Step 2: Restore Kiota tool
echo "[2/4] Restoring Kiota tool..."
dotnet tool restore --tool-manifest "$ROOT_DIR/dotnet-tools.json" -q

# Helper: run Kiota, suppressing stderr only when KIOTA_QUIET=1
executar_kiota() {
    if [ "${KIOTA_QUIET:-0}" = "1" ]; then
        dotnet kiota generate "$@" 2>/dev/null
    else
        dotnet kiota generate "$@"
    fi
}

# Step 3: Generate SDKs
echo "[3/4] Generating SDKs..."

echo "      -> Python..."
executar_kiota \
    --language python \
    --openapi "$OPENAPI_SPEC" \
    --output "$ROOT_DIR/sdks/python/bridgebank" \
    --class-name BridgeBankClient \
    --namespace-name bridgebank \
    --clean-output

echo "      -> TypeScript..."
executar_kiota \
    --language typescript \
    --openapi "$OPENAPI_SPEC" \
    --output "$ROOT_DIR/sdks/typescript/src" \
    --class-name BridgeBankClient \
    --namespace-name bridgebank \
    --clean-output

echo "      -> Java..."
executar_kiota \
    --language java \
    --openapi "$OPENAPI_SPEC" \
    --output "$ROOT_DIR/sdks/java/src/main/java/bridgebank" \
    --class-name BridgeBankClient \
    --namespace-name bridgebank \
    --clean-output

# Step 4: Summary
echo "[4/4] Done!"
echo ""
echo "Generated SDKs:"
echo "  Python:     sdks/python/bridgebank/"
echo "  TypeScript: sdks/typescript/src/"
echo "  Java:       sdks/java/src/main/java/bridgebank/"
echo ""
echo "Install dependencies:"
echo "  Python:     pip install -r sdks/python/requirements.txt"
echo "  TypeScript: cd sdks/typescript && npm install"
echo "  Java:       cd sdks/java && mvn install"
