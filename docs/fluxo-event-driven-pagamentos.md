# Fluxo Event-Driven de Pagamentos

Este documento descreve, de ponta a ponta, como o fluxo de pagamentos foi implementado de forma event-driven no projeto, detalhando os componentes envolvidos, a ordem das operações e como os dados são persistidos.

## Visão Geral

1. A API recebe um `PagamentoDTO` e delega a operação ao `PagamentoService`.
2. O serviço grava o agregado `Pagamento` por meio do `PagamentoRepository` e confirma a transação com `UnitOfWork`.
3. A cada marco do fluxo (início, processamento, conclusão) o serviço publica um evento específico.
4. Os eventos são serializados pelo `EventPublisher` e persistidos na tabela `StoredEvent` por meio do `EventStoreRepository`.
5. Uma Azure Function é invocada para notificar o cliente sobre o status do pagamento; sua resposta é registrada no evento de conclusão.

## Componentes Principais

| Camada | Tipo | Responsabilidade |
|--------|------|------------------|
| `FCG.Application` | Serviço (`PagamentoService`) | Orquestra o fluxo de pagamento, executa persistência primária, chama integrações externas e dispara eventos. |
| `FCG.Domain` | Entidades e Eventos (`Pagamento`, `Event`, `PagamentoIniciadoEvent`, `PagamentoProcessandoEvent`, `PagamentoConcluidoEvent`, `StoredEvent`) | Representam os dados e marcos do domínio. `Event` fornece metadados comuns e `StoredEvent` modela a entrada na Event Store. |
| `FCG.Domain.Interfaces` | Repositórios (`IPagamentoRepository`, `IEventStoreRepository`) e `IEventPublisher` | Contratos para persistência e publicação de eventos. |
| `FCG.Infra.Data` | Implementações (`PagamentoRepository`, `EventStoreRepository`, `EventPublisher`) | Persistem o pagamento e o histórico de eventos utilizando `ApplicationDbContext`. |
| `FCG.Infra.Ioc` | `DependencyInjection` | Registra dependências de serviços, repositórios e publisher para que o fluxo funcione em runtime. |

## Fluxo Detalhado

### 1. Recebimento da requisição e mapeamento

- O controller converte o payload em `PagamentoDTO` e chama `PagamentoService.Efetuar`.
- O serviço gera um `correlationId` para rastrear a requisição de ponta a ponta.
- O DTO é transformado em entidade `Pagamento` com AutoMapper, respeitando o padrão de isolamento entre camadas.

### 2. Persistência da entidade de pagamento

- O método `PagamentoRepository.Efetuar` adiciona a entidade ao `ApplicationDbContext`.
- `UnitOfWork.Commit` executa `SaveChangesAsync`, garantindo atomicidade da escrita.
- Um `PagamentoViewModel` é produzido para retorno seguro ao consumidor.

### 3. Publicação do `PagamentoIniciadoEvent`

- Assim que o pagamento é persistido, o serviço dispara `PagamentoIniciadoEvent`, contendo IDs de usuário, jogo, forma de pagamento, valor e quantidade.
- O evento herda de `Event`, incorporando `CorrelationId`, `AggregateId` e `Timestamp`.
- `EventPublisher.PublishAsync` serializa o evento em JSON, calcula a próxima versão via `EventStoreRepository.GetNextVersionAsync` e insere o registro em `StoredEvent`.

### 4. Publicação do `PagamentoProcessandoEvent`

- O serviço busca dados complementares (`ObterDetalhesPagamento`) para compor a notificação.
- Um segundo evento é emitido com o status "NotificandoServicoExterno" e dados de contato (e-mail) que serão usados pela Azure Function.
- O mesmo pipeline (`EventPublisher` + `EventStoreRepository`) garante que o evento seja gravado na sequência correta.

### 5. Integração com Azure Function

- Se os detalhes do pagamento e a URL estiverem disponíveis, o serviço realiza um `POST` com `HttpClient`.
- Sucesso: lê `PagamentoResponse` e registra mensagem da função.
- Falha: adiciona notificações de aviso e registra log estruturado sem interromper o fluxo.

### 6. Publicação do `PagamentoConcluidoEvent`

- Inclui o resultado da tentativa de notificação (`notificacaoRealizada`) e a mensagem final (sucesso ou erro da Azure Function).
- O evento é persistido na tabela `StoredEvent`, fechando o ciclo auditável do pagamento.

## Persistência de Eventos

- A classe `StoredEvent` grava `EventType`, `AggregateId`, `Data` (payload JSON), `Timestamp` e `Version`.
- A versão é incremental por agregado, permitindo reconstruir a sequência temporal de cada pagamento.
- A tabela `StoredEvent` pode ser consumida por projeções, reprocessamento ou auditoria de conformidade.

## Conceitos-Chave Envolvidos

- **Event Sourcing**: cada mudança relevante gera um evento imutável registrado na Event Store, possibilitando reconstrução de estado e auditoria.
- **Event-Driven Architecture**: o serviço publica eventos que podem ser consumidos por outras partes do sistema (projeções, integrações externas) sem acoplamento direto.
- **Unit of Work**: garante que a escrita da entidade de pagamento ocorra de forma transacional antes de os eventos subsequentes serem emitidos.
- **Correlation ID**: identifica todas as mensagens associadas a uma mesma requisição, facilitando rastreamento e troubleshooting.
- **Resiliência de integrações**: chamadas à Azure Function são encapsuladas em `try/catch`, convertendo falhas em notificações ao invés de abortar o processo.
- **Logging Estruturado**: mensagens com placeholders fornecem observabilidade completa sobre cada passo da operação.

## Fluxo Resumido em Sequência

1. `PagamentoService.Efetuar` recebe o DTO.
2. Persistência do pagamento via `PagamentoRepository` + `UnitOfWork`.
3. Publicação do `PagamentoIniciadoEvent`.
4. Obtenção de detalhes e publicação do `PagamentoProcessandoEvent`.
5. Chamada (opcional) da Azure Function.
6. Publicação do `PagamentoConcluidoEvent` com o resultado da integração.

Cada etapa é registrada tanto em log quanto na Event Store, garantindo rastreabilidade e auditabilidade completas.

