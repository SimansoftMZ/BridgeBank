FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY src/BridgeBank.Core/BridgeBank.Core.csproj src/BridgeBank.Core/
COPY src/BridgeBank.Parsers/BridgeBank.Parsers.csproj src/BridgeBank.Parsers/
COPY src/BridgeBank.Generators/BridgeBank.Generators.csproj src/BridgeBank.Generators/
COPY src/BridgeBank.ML/BridgeBank.ML.csproj src/BridgeBank.ML/
COPY src/BridgeBank.Api/BridgeBank.Api.csproj src/BridgeBank.Api/

RUN dotnet restore src/BridgeBank.Api/BridgeBank.Api.csproj

COPY src/ src/
COPY README.md .

WORKDIR /src/src/BridgeBank.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Simansoft.BridgeBank.Api.dll"]
