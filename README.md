<img width="641" height="436" alt="Arquitetura teste BTG" src="https://github.com/user-attachments/assets/7096a663-e6e7-4408-8523-b7dd7fff0923" />

# btg-teste-locadora

Sistema para gerenciar os empréstimos dos seus jogos. Permite a inserção/edição/exclusão de amigos e jogos, além do gerenciamento e visualização dos jogos, dos amigos e de qual jogo está com quem.

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
| **Locadora.Application** | Casos de uso (services), DTOs, mapeamentos e interfaces. Orquestra as regras de negócio. |
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

