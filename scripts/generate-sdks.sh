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

# Step 3: Generate SDKs
echo "[3/4] Generating SDKs..."

echo "      -> Python..."
dotnet kiota generate \
    --language python \
    --openapi "$OPENAPI_SPEC" \
    --output "$ROOT_DIR/sdks/python/bridgebank" \
    --class-name BridgeBankClient \
    --namespace-name bridgebank \
    --clean-output 2>/dev/null

echo "      -> TypeScript..."
dotnet kiota generate \
    --language typescript \
    --openapi "$OPENAPI_SPEC" \
    --output "$ROOT_DIR/sdks/typescript/src" \
    --class-name BridgeBankClient \
    --namespace-name bridgebank \
    --clean-output 2>/dev/null

echo "      -> Java..."
dotnet kiota generate \
    --language java \
    --openapi "$OPENAPI_SPEC" \
    --output "$ROOT_DIR/sdks/java/src/main/java/bridgebank" \
    --class-name BridgeBankClient \
    --namespace-name bridgebank \
    --clean-output 2>/dev/null

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
