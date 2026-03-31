# Roadmap do BridgeBank

## Versão 0.5.0 (Actual) 🚧

- [x] Motor de reconciliação com estratégias configuráveis:
  - [x] Por referência exacta
  - [x] Por valor e data
  - [x] Por descrição (similaridade)
  - [x] Baseada em ML (ML.NET)
- [x] Classificação de transacções:
  - [x] Sistema baseado em regras
  - [x] Classificação com ML.NET
  - [x] Treino com dados sintéticos
- [x] Leitores de extractos Excel:
  - [x] BCI
  - [x] Millennium BIM
  - [x] Standard Bank
  - [x] NedBank
  - [x] Access Bank
  - [x] M-Pesa
- [x] Geradores de ficheiros de pagamento:
  - [x] BCI (TXT)
  - [x] Millennium BIM (CSV)
  - [x] Standard Bank (XML)
- [x] API REST (ASP.NET Core Minimal API) com OpenAPI e Scalar
- [x] SDKs tipados via Kiota (Python, TypeScript, Java)
- [x] Docker e Docker Compose
- [x] CI/CD com GitHub Actions (build, testes, publicação NuGet e Docker Hub)
- [x] Testes unitários
- [x] Documentação e exemplos de utilização

## Versão 0.6.0 (Próxima) 📋

- [ ] Leitores de extractos em PDF
- [ ] Suporte para mais bancos moçambicanos:
  - [ ] Absa Bank
  - [ ] First National Bank
  - [ ] Banco Terra
- [ ] Histórico de reconciliações anteriores para melhorar correspondências
- [ ] Interface gráfica web (opcional)

## Versão 0.7.0 (Futuro) 🔮

- [ ] Suporte para bancos internacionais
- [ ] Importação/Exportação em formatos OFX, QIF
- [ ] Relatórios de reconciliação
- [ ] Dashboard de análise
- [ ] Detecção automática de taxas e comissões
- [ ] Suporte para múltiplas moedas

## Versão 1.0.0 (Visão de Longo Prazo) 🌟

- [ ] Reconciliação em tempo real
- [ ] Integração com ERPs populares (Primavera, SAP Business One, Sage)
- [ ] Aplicação mobile
- [ ] Cloud hosting opcional
- [ ] Multi-tenancy

## Como Contribuir

Quer ajudar a implementar alguma destas features? Veja [CONTRIBUTING.md](CONTRIBUTING.md) para começar!

## Sugestões

Tem ideias para o roadmap? Abra uma [issue](https://github.com/SimansoftMZ/BridgeBank/issues) com a label `enhancement`!
