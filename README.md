<img width="641" height="436" alt="Arquitetura teste BTG" src="https://github.com/user-attachments/assets/7096a663-e6e7-4408-8523-b7dd7fff0923" />

# btg-teste-locadora

Sistema para gerenciar os empréstimos dos seus jogos. Permite a inserção/edição/exclusão de amigos e jogos, além do gerenciamento e visualização dos jogos, dos amigos e de qual jogo está com quem.

## 🌐 Ambiente publicado

A aplicação está publicada no Azure App Service:

| Serviço | URL |
|---------|-----|
| **Front-end (Web)** | https://front-btg-locadora-vinicius-bispo-bah8dkarebewcgf8.westeurope-01.azurewebsites.net/ |
| **API REST (Swagger)** | https://api-btg-locadora-vinicius-bispo-fcgsfracdpamccbs.westeurope-01.azurewebsites.net/swagger/index.html |

Nesse ambiente, `ConnectionStrings`, `Jwt:SecretKey` e `AdminCredenciais` **não** vêm dos `appsettings.json` do repositório — são configurados como *Application Settings* (secrets) diretamente no Azure App Service, que o ASP.NET Core carrega como variáveis de ambiente e sobrepõe automaticamente aos valores do `appsettings.json`. Os valores versionados no repositório servem apenas para rodar o projeto localmente (via `dotnet run` ou `docker compose`).

## 📦 Estrutura do repositório

O repositório é dividido em dois projetos independentes, um por camada de apresentação:

```
btg-teste-locadora/
├── btg-locadora-api/   # API REST (.NET 10)
└── btg-locadora-web/   # Front-end (.NET 10)
```

## btg-locadora-api

Projeto em **.NET 10** seguindo **Clean Architecture**, dividido em quatro camadas:

| Camada | Responsabilidade |
|--------|------------------|
| **Locadora.Api** | Controllers, validação (FluentValidation) e configuração da API. |
| **Locadora.Application** | Casos de uso via **CQRS** (Commands/Queries e handlers com MediatR), DTOs, mapeamentos e interfaces. Orquestra as regras de negócio. |
| **Locadora.Domain** | Entidades, enums e contratos de repositório. Sem dependências externas. |
| **Locadora.Infrastructure** | Acesso a dados (EF Core) e implementação dos repositórios. |


```
btg-locadora-api/
├── src/
│   └── Locadora.Api
│   ├── Locadora.Application
│   ├── Locadora.Domain
│   ├── Locadora.Infrastructure
└── tests/
    └── Locadora.Application.UnitTest
```

### Tecnologias

- **.NET 10** / ASP.NET Core
- **Entity Framework Core** + **SQL Server**
- **Redis** (cache)
- **FluentValidation** (validação na entrada da API)
- **Swagger** (documentação)
- **xUnit**, **Moq** e **FluentAssertions** (testes)

### CQRS

A `Locadora.Application` separa **Commands** (escrita) e **Queries** (leitura) usando **MediatR**, cada caso de uso isolado em sua própria pasta:

```
Locadora.Application/
├── Commands/
│   └── {Agregado}/{CasoDeUso}/
│       ├── {CasoDeUso}Command.cs         # record IRequest<TResult>
│       └── {CasoDeUso}CommandHandler.cs  # IRequestHandler<TCommand, TResult>
└── Queries/
    └── {Agregado}/{CasoDeUso}/
        ├── {CasoDeUso}Query.cs
        └── {CasoDeUso}QueryHandler.cs
```

Essa separação também existe na camada de persistência, com **DbContexts e repositórios dedicados** para cada lado:

| | Escrita (Commands) | Leitura (Queries) |
|---|---|---|
| **DbContext** | `LocadoraDbContext` | `LocadoraReadDbContext` (`QueryTrackingBehavior.NoTracking`) |
| **Repositórios** | `I{Entidade}Repository` → `Persistence/Repositories/Command/{Entidade}Repository.cs` | `I{Entidade}QueryRepository` → `Persistence/Repositories/Query/{Entidade}QueryRepository.cs` (`AsNoTracking()`) |
| **Usado por** | Command handlers | Query handlers |

Os dois `DbContext` compartilham as mesmas `Configurations/*Configuration.cs` (`ApplyConfigurationsFromAssembly`) e, hoje, apontam para a **mesma** connection string (`ConnectionStrings:SqlServerDB`). A estrutura já permite evoluir para bases físicas diferentes (ex.: um replica de leitura) bastando apontar `LocadoraReadDbContext` para outra connection string — sem precisar alterar Commands, Queries ou handlers.

## btg-locadora-web

Projeto em **.NET 10** (ASP.NET Core MVC) seguindo o mesmo estilo de Clean Architecture da API, dividido em quatro camadas:

| Camada | Responsabilidade |
|--------|------------------|
| **Locadora.Web** | Controllers MVC, Views (Razor), ViewModels e configuração da aplicação (autenticação por cookie, DI). |
| **Locadora.Web.Application** | Interfaces (`I*Service`, `I*ApiClient`) e services que orquestram as chamadas à API. |
| **Locadora.Web.Domain** | Modelos, DTOs e exceptions. Sem dependências externas. |
| **Locadora.Web.Infrastructure** | Clients HTTP (`HttpClient` tipado) que consomem os endpoints da `Locadora.Api`, incluindo o handler que injeta o Bearer token do usuário autenticado. |

```
btg-locadora-web/
├── src/
│   └── Locadora.Web
│   ├── Locadora.Web.Application
│   ├── Locadora.Web.Domain
│   ├── Locadora.Web.Infrastructure
```

### Funcionalidades

- Login (JWT obtido via `POST /api/Autenticacao/login`, armazenado em cookie de autenticação da aplicação).
- CRUD de Amigos e Jogos.
- Registro e devolução de Empréstimos, com filtros por jogo/amigo/status.
- Seleção de Gêneros, Desenvolvedores e Publicadoras no formulário de Jogos via autocomplete (busca por texto, mín. 3 letras), consumindo os endpoints correspondentes da API.

### Tecnologias

- **.NET 10** / ASP.NET Core MVC
- **Newtonsoft.Json** para (de)serialização das chamadas HTTP
- **Bootstrap 5** + **jQuery** (validação client-side)

## 🐳 Ambiente com Docker Compose

A partir da raiz do repositório (onde está o `docker-compose.yml`):

```bash
# Sobe toda a stack (--build garante o rebuild quando o código muda)
docker compose up --build

# Parar a stack
docker compose down
```

| Serviço                | URL / Endpoint                          | Porta (host → container) | Observação |
|-------------------------|------------------------------------------|---------------------------|------------|
| **Front-end (Web)**     | http://localhost:8081                    | `8081 → 8080`              | Login: `admin` / `Admin@123` |
| **API REST**            | http://localhost:8080                    | `8080 → 8080`               | Swagger em `/swagger` |
| **SQL Server**          | `localhost,1433`                         | `1433 → 1433`               | User: `sa` / Senha: `Str0ngP@ssw0rd!` / DB: `DB_LOCADORA` |
| **Redis**               | `localhost:6379`                         | `6379 → 6379`               | Cache distribuído |

