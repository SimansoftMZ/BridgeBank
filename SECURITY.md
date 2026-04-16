# Política de Segurança

Este documento descreve as práticas de segurança, procedimentos de divulgação responsável e orientações para mitigar riscos ao utilizar o BridgeBank.

## Reportar Vulnerabilidades

Se descobrir uma vulnerabilidade de segurança, **não abra uma issue pública no GitHub**. Em vez disso, reporte-a de forma confidencial através de um dos seguintes canais:

### Via GitHub Security Advisory

1. Aceda a [GitHub Security Advisory](https://github.com/SimansoftMZ/BridgeBank/security/advisories)
2. Clique em **"Report a vulnerability"**
3. Descreva a vulnerabilidade com detalhes técnicos

### Via Email (Segurança)

Envie um email para a equipa de segurança do projecto com:

- Descrição clara da vulnerabilidade
- Passos para reproduzir
- Impacto potencial
- Versões afectadas
- Sugestões de correcção (se possível)

**A segurança é uma prioridade — as vulnerabilidades reportadas serão tratadas com urgência.**

## Processo de Divulgação

1. **Recepção:** Acknowledgeamos a recepção do relatório dentro de 48 horas
2. **Avaliação:** A equipa avalia a vulnerabilidade e o seu impacto
3. **Correcção:** Desenvolvemos uma correcção em segredo numa branch privada
4. **Teste:** A correcção é validada e testada completamente
5. **Release:** Uma nova versão é publicada com a correcção
6. **Divulgação:** Um CVE (Common Vulnerabilities and Exposures) é atribuído, se aplicável
7. **Comunicação:** O relatório original recebe crédito (a menos que prefira anonimato)

**Prazo esperado:** 30 dias entre a reportação e a divulgação pública.

## Responsabilidades de Segurança

### Utilizadores da Biblioteca

Ao utilizar o BridgeBank, é responsabilidade sua:

- **Manter dependências atualizadas** — Configure alertas de segurança do GitHub (Dependabot)
- **Validar dados de entrada** — O BridgeBank processa dados bancários; sempre valide ficheiros e entrada do utilizador
- **Proteger credenciais** — Nunca inclua tokens de API, senhas ou chaves de acesso no controlo de versão
- **Usar HTTPS em produção** — Se utilizar a API REST, sempre use conexões encriptadas
- **Auditar acessos** — Quem tem acesso aos dados processados pelo BridgeBank?

### Utilizadores da API REST

Se executar a API REST em produção:

- **Autenticação e Autorização** — Configure controles de acesso (ex: OAuth 2.0, JWT)
- **Limite de Taxa** — Implemente rate limiting para prevenir abuso
- **HTTPS/TLS** — Use certificados SSL/TLS válidos
- **Validação de Upload** — A API aceita uploads de ficheiros — valide tipo, tamanho e conteúdo
- **Monitoramento** — Monitore logs e métricas para atividade suspeita

### Contribuidores

Se contribuir para o projecto:

- **Não envie segredos** — Revise seus commits antes de push; evite expor chaves, tokens ou dados sensíveis
- **Testes de segurança** — Teste casos extremos e entrada malformada
- **Dependências** — Use versões knowns de bibliotecas; revise `csproj` para dependências desatualizadas
- **Code review** — Aceite feedback crítico sobre segurança

## Responsabilidades de Segurança do Projecto

A Simansoft é responsável por:

- **Auditar dependências regularmente** — Verificar vulnerabilidades conhecidas
- **Aplicar patches** — Corrigir vulnerabilidades reportadas rapidamente
- **Comunicar riscos** — Publicar avisos de segurança quando necessário
- **Revisar código** — Validar mudanças antes do merge
- **Testes de segurança** — Executar testes de segurança estática (SAST) na CI/CD

## Problemas de Segurança Conhecidos

Não há vulnerabilidades conhecidas no momento da última atualização. Consulte as [GitHub Security Advisories](https://github.com/SimansoftMZ/BridgeBank/security/advisories) para informações atualizadas.

## Boas Práticas para Dados Sensíveis

### Extractos Bancários

- **Armazenamento** — Criptografe ficheiros de extracto em repouso (ex: AES-256)
- **Transmissão** — Use HTTPS/TLS ao enviar extractos para a API
- **Logs** — Não registre IBANs, números de contas ou valores sensíveis em logs
- **Temporário** — Delete ficheiros extracto após processamento

### Números de Contas Bancárias

- **Validação** — Valide o formato (ex: IBAN) antes de processar
- **Mascaramento** — Nos logs/UI, mostre apenas os últimos 4 dígitos
- **Encriptação** — Criptografe dados sensíveis em repouso e em trânsito

### Transacções

- **Acesso mínimo** — Apenas utilizadores autenticados acedem a dados de transacção
- **Auditoria** — Registre quem acedeu a que dados e quando
- **Retenção** — Defina políticas de retenção de dados (ex: apagar após 7 anos)

## Autenticação na API

A API REST não impõe autenticação por padrão. **Para produção, configure:**

```csharp
// Exemplo: Adicionar autenticação JWT na API
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://seu-provedor-identidade.com";
        options.Audience = "bridgebank-api";
    });

app.UseAuthentication();
app.UseAuthorization();
```

## Validação de Entrada

O BridgeBank processa ficheiros Excel e JSON. Implemente validações:

```csharp
// Validar tamanho do ficheiro
const long maxFileSize = 10 * 1024 * 1024; // 10 MB
if (file.Length > maxFileSize)
    throw new InvalidOperationException("Ficheiro demasiado grande");

// Validar tipo MIME
var allowedTypes = new[] { "application/vnd.ms-excel", 
                           "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" };
if (!allowedTypes.Contains(file.ContentType))
    throw new InvalidOperationException("Tipo de ficheiro não permitido");

// Validar conteúdo
var extrato = leitor.LerExtrato(file.FileName);
if (extrato?.Transacoes == null || !extrato.Transacoes.Any())
    throw new InvalidOperationException("Extracto vazio ou inválido");
```

## Gestão de Dependências

O BridgeBank utiliza as seguintes dependências principais:

| Dependência | Versão | Propósito | Risco |
|------------|--------|----------|-------|
| `NPOI` | ≥ 2.6.0 | Leitura de ficheiros Excel | ⚠️ Atualizar regularmente |
| `ML.NET` | ≥ 4.0.0 | Classificação com ML | ⚠️ Auditar modelos |
| `SharpCompress` | (if used) | Compressão | ⚠️ Vulnerabilidades conhecidas — usar `System.IO.Compression` |

**Recomendação:** Configure [Dependabot](https://github.com/SimansoftMZ/BridgeBank/security/dependabot) no GitHub para alertas automáticos.

## Testes de Segurança

Para testar a segurança localmente:

```bash
# Análise estática de código (usando Roslyn Analyzers)
dotnet build /p:EnforceCodeStyleInBuild=true

# Verificar dependências vulneráveis
dotnet list package --vulnerable

# Executar testes com inputs malformados
dotnet test tests/BridgeBank.Core.Tests --filter "Security"
```

## Encriptação em Trânsito

Se utilizar a API REST, configure HTTPS obrigatório:

```csharp
// Program.cs
app.UseHsts(); // HTTP Strict-Transport-Security
app.UseHttpsRedirection();

// Certificado SSL (produção)
// Use um certificado válido, ex: Let's Encrypt
```

No Docker:

```yaml
# docker-compose.yml
services:
  api:
    environment:
      - ASPNETCORE_HTTPS_PORT=443
      - ASPNETCORE_URLS=https://+:443;http://+:80
    ports:
      - "443:443"
    volumes:
      - ./certs:/https:ro  # Montar certificados
```

## Criptografia em Repouso

Para dados bancários em repouso:

```csharp
using System.Security.Cryptography;
using System.Text;

public class CriptografiaDados
{
    public static string Encriptar(string dados, byte[] chave)
    {
        using var aes = Aes.Create();
        aes.Key = chave;
        aes.GenerateIV();
        
        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        ms.Write(aes.IV, 0, aes.IV.Length);
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            cs.Write(Encoding.UTF8.GetBytes(dados));
        }
        
        return Convert.ToBase64String(ms.ToArray());
    }
}
```

## Logs e Monitoramento

### O Que Registrar

✅ **Seguro:**
- Erros de validação (estrutura, não valores)
- Falhas de autenticação (IP, timestamp)
- Operações bem-sucedidas (quem, quando, que recurso)

❌ **Nunca Registre:**
- IBANs, números de contas, cartões
- Valores de transacções sensíveis
- Senhas, tokens de API, chaves criptográficas

### Exemplo Seguro

```csharp
_logger.LogWarning("Falha ao processar extracto para banco {Banco} (tipo de ficheiro: {Tipo})", 
    banco, tipo);

// NÃO:
_logger.LogWarning("Falha ao processar extracto: {Extracto}", extractoJson);
```

## Auditoria

Implemente logs de auditoria para conformidade:

```csharp
public class LogAuditoria
{
    public int Id { get; set; }
    public string Utilizador { get; set; }
    public string Acao { get; set; }           // "Processou extracto", "Gerou pagamento"
    public string Recurso { get; set; }        // ID do extracto, não dados sensíveis
    public DateTime Timestamp { get; set; }
    public string EnderecoIP { get; set; }
    public string Resultado { get; set; }      // "Sucesso", "Falha"
}
```

## Conformidade

### Protecção de Dados Pessoais

Se o seu sistema processa dados pessoais (ex: NUIT, nomes), cumpra:

- **LEI MOÇAMBICANA DE PROTEÇÃO DE DADOS** (se aplicável)
- **RGPD** (se utilizadores da EU)
- **PCI DSS** (se processar dados de cartão)

### Conformidade Bancária

Em Moçambique, bancos estão sujeitos a:

- **Normas BdM** (Banco de Moçambique)
- **Lei de Segurança Informática**
- **Controlos internos e auditoria**

Consulte a legislação local antes de implementar em produção.

## Atualizações de Segurança

Subscreva notificações de segurança:

1. **GitHub:** [Watch Releases](https://github.com/SimansoftMZ/BridgeBank/releases) — selecione "Releases only"
2. **NuGet:** Alertas automáticos para pacotes desatualizados
3. **Email:** (Quando disponível) Inscreva-se na mailing list de segurança

## Contacto de Segurança

Para questões de segurança confidenciais, entre em contacto com:

- **Email:** [segurança do projecto]  
- **GitHub Security Advisory:** [Reportar via GitHub](https://github.com/SimansoftMZ/BridgeBank/security/advisories)

## Histórico de Versões de Segurança

| Versão | Assunto | Data |
|--------|---------|------|
| 1.0.0+ | Versão inicial | [data] |

*(Atualizaremos conforme novas versões forem lançadas)*

## Referências

- [OWASP Top 10](https://owasp.org/Top10/)
- [Microsoft .NET Security Best Practices](https://docs.microsoft.com/en-us/dotnet/fundamentals/security/)
- [PCI DSS Compliance Guide](https://www.pcisecuritystandards.org/)
- [Banco de Moçambique - Normas de Segurança](https://www.bancomocambique.mz/)

---

**Última atualização:** 2026-04-16  
**Versão:** 1.0
