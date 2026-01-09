# Guia de Contribuição

Obrigado por considerar contribuir para o BridgeBank! 🎉

## Como Contribuir

### Reportar Bugs

Se encontrar um bug, por favor abra uma [issue](https://github.com/SimansoftMZ/BridgeBank/issues) incluindo:

- Descrição clara do problema
- Passos para reproduzir
- Comportamento esperado vs. atual
- Versão da biblioteca
- Ambiente (.NET version, OS)

### Sugerir Melhorias

Sugestões são bem-vindas! Abra uma issue com:

- Descrição detalhada da melhoria
- Casos de uso
- Exemplos de como seria usado

### Submeter Pull Requests

1. **Fork o repositório**
2. **Clone seu fork**
   ```bash
   git clone https://github.com/SEU-USUARIO/BridgeBank.git
   ```
3. **Crie uma branch**
   ```bash
   git checkout -b feature/minha-feature
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

8. **Abra um Pull Request**

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
  namespace BridgeBank.Core.Models
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
