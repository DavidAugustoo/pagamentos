# 🕹️ FiapCloudGames

### Plataforma de Gestão de Jogos Digitais e Servidores para partidas online

O projeto **FiapCloudGames** é a primeira fase de um sistema de gerenciamento de jogos digitais, desenvolvido como parte do **Tech Challenge** da FIAP. Nesta etapa inicial, foi construída uma **API REST** utilizando **.NET 8**, com foco em boas práticas de desenvolvimento, arquitetura limpa, segurança e escalabilidade.

---

## 🎯 Objetivo

Desenvolver uma API para:
- Cadastro e autenticação de usuários
- Gerenciamento da biblioteca de jogos adquiridos
- Fornecer base sólida para futuras fases do projeto, como gerenciamento de jogos e servidores para partidas Online

---
## 🧰 Instruções de Uso

### ✅ Pré-requisitos
Certifique-se de ter os seguintes softwares instalados na máquina:

- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download)
- [SQL Server](https://www.microsoft.com/pt-br/sql-server/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/pt-br/)

---
### 🚀 A API está hospedada na Microsoft Azure
1. **Acesse a API em: https://fiapcloudgameswebapi-dfaeh8cxf5h0feb6.westus-01.azurewebsites.net/swagger/index.html**
  
---
### 🚀 Como rodar a API localmente

1. **Clone o repositório**
2. **Abra o projeto com o Visual Studio 2022**
3. **Configure a conexão com o banco de dados em: appsettings.Development.json**
4. **Execute as migrations caso o banco de dados não exista com o comando: dotnet ef database update**
5. **Defina o projeto FCG.API como Projeto de Inicialização**
6. **Execute a aplicação pressionando F5 ou execute o comando: dotnet run --project FCG.API**
7. **Após iniciar a aplicação, acesse o Swagger para visualizar e testar os endpoints: https://localhost:7188/swagger/index.html**
8. **Para executar o projeto de testes clique com o botão direito em FCG.Tests e Executar Testes**

---

## 📚 Tecnologias Utilizadas

- **.NET 8** – Framework principal utilizado para construção da API REST, com alto desempenho, segurança e escalabilidade.
- **Entity Framework Core** – ORM para mapeamento objeto-relacional e controle de persistência de dados via Migrations.
- **SQL Server** – Banco de dados relacional utilizado para armazenar os dados da aplicação com consistência e integridade.
- **AutoMapper** – Biblioteca para mapeamento automático entre entidades de domínio, DTOs e ViewModels, promovendo desacoplamento entre camadas.
- **JWT (JSON Web Token)** – Mecanismo de autenticação e autorização segura, com tokens assinados e expiração controlada.
- **Swagger (OpenAPI)** – Ferramenta para documentação automática e interativa dos endpoints da API, com suporte a autenticação via Bearer Token.
- **xUnit** – Framework de testes utilizado para validar as regras de negócio por meio de testes unitários.
- **Clean Architecture + Domain-Driven Design (DDD)** – Padrões arquiteturais que garantem separação de responsabilidades, modularidade, coesão e fácil manutenção do código.
- **Injeção de Dependência (IoC)** – Implementada com `IServiceCollection` para promover baixo acoplamento entre componentes e facilitar a testabilidade.
- **Middleware de tratamento de erros** – Captura global de exceções com retorno estruturado e integração com logs.
- **Paginação customizada** – Implementada para controle de grandes volumes de dados em endpoints de listagem com filtros dinâmicos.
- **Data Dog** - Ferramenta de observabilidade e monitoramento da aplicação que permite acompanhar o desempenho, logs, métricas e traces em tempo real.
- **Docker** - Plataforma para empacotar a aplicação com todas as dependências, ambiente de execução e configurações em um container.
- **Microsoft Azure** - Plataforma de computação em nuvem que oferece infraestrutura e serviços para desenvolver, hospedar, escalar e gerenciar a aplicação sem precisar manter servidores físicos.
---

## 🧱 Arquitetura

O projeto segue os princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**, promovendo a separação clara de responsabilidades, baixo acoplamento e alta coesão entre os módulos.

```
FCG.API
├─ Camada de apresentação da aplicação
├─ Controllers REST
├─ Middleware global de tratamento de erros
├─ Integração com Swagger para documentação
├─ Implementação de paginação e segurança JWT

FCG.Application
├─ Serviços de aplicação (Application Services)
├─ DTOs e ViewModels para comunicação entre camadas
├─ Interfaces que definem os contratos de uso
├─ Mapeamentos com AutoMapper
├─ Lógica de orquestração da aplicação (sem lógica de domínio)

FCG.Domain
├─ Entidades do núcleo de negócio com encapsulamento rico (Rich Domain)
├─ Value Objects imutáveis e autoconsistentes (ex: Email)
├─ Interfaces de repositórios (contratos de infraestrutura)
├─ Validações e exceções de domínio
├─ Constantes e mensagens centralizadas
├─ Padrões de notificação para retorno estruturado de mensagens ou erros

FCG.Infra.Data
├─ Implementações dos repositórios (Repository Pattern)
├─ Contexto de banco com Entity Framework Core
├─ Configuração e aplicação de Migrations
├─ Unit of Work para gerenciamento transacional
├─ Configurações específicas do EF Core (ModelBuilder, Fluent API)

FCG.Infra.Ioc
├─ Registro de dependências (Injeção de Dependência)
├─ Configuração de autenticação JWT
├─ Integração com serviços como AutoMapper, Swagger e EF Core

FCG.Tests
├─ Testes unitários 
├─ Desenvolvimento guiado por testes
```
---

## ✅ Funcionalidades Implementadas

### 👤 Módulo de Usuários

- 📌 **CRUD completo de usuários**, com suporte a inclusão, edição, exclusão e consulta detalhada.
- 📬 **Recuperação de senha segura**, com geração de senha temporária e atualização criptografada utilizando HMACSHA512.
- 🔐 **Autenticação via JWT**, com validação completa do token (assinatura, emissor, audiência e expiração).
- 🛡️ **Proteção de rotas por perfil**, com controle de acesso baseado em roles (`Admin`, `Usuário`) aplicando `[Authorize(Roles = "...")]`.
- 📥 **Validação rigorosa de senha segura**, exigindo no mínimo 8 caracteres, incluindo letras maiúsculas, minúsculas, números e caracteres especiais.
- 📩 **Validação de formato de e-mail** e encapsulamento com Value Object para garantir integridade dos dados.
- 📊 **Paginação customizada nas buscas**, otimizando performance e organização de grandes volumes de usuários.
- 🧾 **Retorno padronizado com Notifications Result**, facilitando o tratamento de erros e mensagens de validação em toda a aplicação.
- 🔄 **Respostas consistentes via ViewModels**, garantindo clareza e segurança nas respostas enviadas ao frontend.

---

## 🔧 Principais Implementações

- ✅ **Entity Framework Core** com suporte a **Migrations automáticas**, garantindo versionamento e controle do esquema do banco de dados relacional.
- ✅ **Unit of Work** para orquestrar transações de forma centralizada, assegurando **consistência e atomicidade** nas operações de escrita.
- ✅ **Repository Pattern** implementado com interfaces de domínio para **abstração da lógica de acesso a dados**, promovendo testabilidade e separação de responsabilidades.
- ✅ **Value Objects**, como o `Email`, modelados conforme os princípios de **Domain-Driven Design**, encapsulando validações e comportamentos imutáveis de atributos de valor.
- ✅ **Middleware global de tratamento de erros**, com logging estruturado e resposta padronizada para falhas em tempo de execução.
- ✅ **DTOs (Data Transfer Objects)** para recebimento e envio de dados via API, e **ViewModels** para apresentação de respostas, garantindo **desacoplamento entre domínio e interface externa**.
- ✅ **Logs estruturados com ILogger**, promovendo rastreabilidade e suporte à observabilidade durante a execução da aplicação.
- ✅ **Autenticação JWT** com validação completa de token (assinatura, expiração, emissor, audiência), incluindo controle de perfis de acesso (`Admin`, `Usuário`).
- ✅ **Proteção de senhas com HMACSHA512** utilizando **salt criptográfico** exclusivo por usuário, armazenando `PasswordHash` e `PasswordSalt` com segurança.
- ✅ **Injeção de Dependência** com `IServiceCollection` e organização centralizada via `DependencyInjection`, facilitando o desacoplamento de componentes e testabilidade.
- ✅ **Documentação da API com Swagger**, incluindo autenticação com `Bearer Token` e suporte a testes interativos dos endpoints.
- ✅ **Paginação customizada** nos endpoints de listagem, com suporte a filtros dinâmicos e ordenação.

## 🧪 Testes e Qualidade

A arquitetura do projeto foi desenhada para facilitar a aplicação de **Testes Unitários** e **Desenvolvimento Orientado a Testes (TDD)**. 

### ✅ Testes Unitários e TDD com xUnit
O objetivo é validar as principais regras de negócio críticas da aplicação e, foram aplicadas para as seguintes regras:

- 🔐 **Validação de senha segura**:
- 👤 **Criação de usuários**:
- 🔑 **Permissões de acesso**:
  
## 📄 Documentação da API

Acesse `https://localhost:7188/swagger/index.html` para visualizar e testar todos os endpoints disponíveis via Swagger.

## 👨‍💻 Autor

**Vinícius Breda Silva**, 
**David Augusto de Andrade Ribeiro**, 
**Lucas Dantas dos Santos** e 
**Nasser Souza Almeida**
