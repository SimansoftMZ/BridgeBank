# Guia de Contribuição

Obrigado por considerar contribuir para o BridgeBank! 🎉

## Como Contribuir

### Reportar Bugs

Se encontrar um bug, por favor abra uma [issue](https://github.com/SimansoftMZ/BridgeBank/issues) incluindo:

- Descrição clara do problema
- Passos para reproduzir
- Comportamento esperado vs. actual
- Versão da biblioteca
- Ambiente (.NET version, OS)

### Sugerir Melhorias

Sugestões são bem-vindas! Abra uma issue com:

- Descrição detalhada da melhoria
- Casos de uso
- Exemplos de como seria usado

### Estratégia de Branches

O projecto segue o modelo **GitHub Flow** com tags para releases:

```
main (protegido)          ← branch estável, sempre pronto para release
  ├── feature/*           ← novas funcionalidades
  ├── fix/*               ← correções de bugs
  ├── docs/*              ← alterações de documentação
  └── chore/*             ← manutenção, CI/CD, dependências
```

**Regras:**
- `main` é protegido — todo o trabalho entra via Pull Request
- PRs requerem CI a passar (build + testes) antes do merge
- Releases são criadas com tags no formato `v*.*.*` (ex: `v1.0.0`)
- Tags disparam publicação automática no NuGet e Docker Hub

### Submeter Pull Requests

1. **Fork o repositório**
2. **Clone seu fork**
   ```bash
   git clone https://github.com/SEU-USUARIO/BridgeBank.git
   ```
3. **Crie uma branch a partir de `main`**
   ```bash
   git checkout -b feature/minha-feature
   # ou: fix/corrigir-parser, docs/actualizar-readme, chore/actualizar-deps
   ```
4. **Faça suas alterações**
   - Siga as convenções de código
   - Adicione testes
   - Atualize documentação

5. **Execute os testes**
   ```bash
   dotnet test
   ```

6. **Commit suas mudanças**
   ```bash
   git commit -m "Adicionar: descrição da feature"
   ```

7. **Push para seu fork**
   ```bash
   git push origin feature/minha-feature
   ```

8. **Abra um Pull Request para `main`**
   - O CI executará build e testes automaticamente
   - Aguarde aprovação de um mantenedor

## Convenções de Código

### Nomenclatura

- **Classes e Métodos**: PascalCase em português
  ```csharp
  public class MotorReconciliacao
  {
      public void ReconciliarTransacoes() { }
  }
  ```

- **Variáveis locais**: camelCase em português
  ```csharp
  var transacaoBancaria = new Transacao();
  ```

- **Projetos e Namespaces**: PascalCase em inglês
  ```csharp
  namespace Simansoft.BridgeBank.Core.Models
  ```

### Documentação

- Todo método público deve ter comentário XML:
  ```csharp
  /// <summary>
  /// Reconcilia transações bancárias com lançamentos ERP
  /// </summary>
  /// <param name="transacoes">Lista de transações bancárias</param>
  /// <returns>Resultados da reconciliação</returns>
  public List<Resultado> Reconciliar(IEnumerable<Transacao> transacoes)
  ```

### Testes

- Nomes de testes descritivos:
  ```csharp
  [Fact]
  public void Reconciliar_ComReferenciaExata_DeveRetornarCorrespondencia()
  ```

- Use padrão AAA (Arrange, Act, Assert):
  ```csharp
  // Arrange
  var motor = new MotorReconciliacao();
  
  // Act
  var resultado = motor.Reconciliar(transacoes, lancamentos);
  
  // Assert
  Assert.NotNull(resultado);
  ```

## CI/CD

### Pipeline de Integração Contínua (CI)

Executado automaticamente em cada PR e push para `main`:
- Restore de dependências
- Build em modo Release
- Execução de todos os testes

### Pipeline de Release

Disparado automaticamente ao criar uma tag `v*.*.*`:
1. Build e testes (validação)
2. Publicação de pacotes NuGet (`Simansoft.BridgeBank.*`)
3. Publicação de imagem Docker (`simansoft/bridgebank-api`)

### Como Criar uma Release

```bash
# Certifique-se de estar no main actualizado
git checkout main
git pull origin main

# Crie e publique a tag
git tag v1.0.0
git push origin v1.0.0
```

### Secrets Necessários (GitHub)

| Secret | Descrição |
|--------|-----------|
| `NUGET_API_PUBLISH_KEY` | API key do NuGet.org para publicar pacotes |
| `DOCKERHUB_USERNAME` | Username da conta Docker Hub |
| `DOCKERHUB_TOKEN` | Access Token do Docker Hub (não a password) |

Para configurar os secrets: **Settings → Secrets and variables → Actions → New repository secret**

#### Obter o Docker Hub Access Token

1. Aceda a [hub.docker.com](https://hub.docker.com) e faça login
2. Clique no seu avatar → **Account settings** → **Personal access tokens**
3. Clique em **Generate new token**
4. Dê um nome descritivo (ex: `github-actions-bridgebank`)
5. Seleccione permissões: **Read & Write**
6. Copie o token gerado e adicione como secret `DOCKERHUB_TOKEN` no GitHub
7. Adicione também o seu username Docker como secret `DOCKERHUB_USERNAME`

## Processo de Revisão

1. Todos os PRs serão revisados por mantenedores
2. Feedback será fornecido
3. Mudanças podem ser solicitadas
4. Após aprovação, será feito merge

## Código de Conduta

- Seja respeitoso
- Aceite feedback construtivo
- Foque no que é melhor para a comunidade

## Licença

Ao contribuir, você concorda que suas contribuições serão licenciadas sob a MIT License.