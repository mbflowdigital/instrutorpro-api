# InstrutorPro API

Backend da plataforma **InstrutorPro** — conecta instrutores de trânsito credenciados pelo Detran a alunos que buscam habilitação, aproveitando as mudanças regulatórias de 2026 que permitem instrutores autônomos.

---

## 📋 Descrição do Projeto

O **InstrutorPro** é um marketplace de instrutores de trânsito autônomos. Os instrutores se cadastram, têm seu credenciamento validado automaticamente via API governamental (Senatran/Detran) e ficam visíveis para alunos buscarem e agendarem aulas práticas.

### Modelo de Negócio
- **Instrutores**: Pagam mensalidade para ficar ativos e visíveis na plataforma
- **Alunos**: Acessam gratuitamente e contratam instrutores diretamente
- **Validação automática**: Double-check semanal via API gov para garantir que só instrutores credenciados ficam ativos

---

## 🏗️ Arquitetura

O projeto segue **Clean Architecture** com separação clara de responsabilidades:

```
src/
├── InstrutorPro.Domain/          # Entidades, Value Objects, Regras de negócio
├── InstrutorPro.Application/     # Use Cases (CQRS/MediatR), DTOs, Interfaces
├── InstrutorPro.Infrastructure/  # EF Core, Repositórios, Integrações externas
└── InstrutorPro.API/             # Controllers, Middlewares, Program.cs

tests/
├── InstrutorPro.UnitTests/       # Testes unitários dos Use Cases
└── InstrutorPro.IntegrationTests/# Testes de integração dos Controllers
```

---

## 🛠️ Stack Utilizada

| Camada | Tecnologia |
|--------|-----------|
| Runtime | .NET 8 / ASP.NET Core |
| ORM | Entity Framework Core 8 |
| Banco de Dados | PostgreSQL (Npgsql) |
| Cache | Redis (StackExchange.Redis) |
| CQRS | MediatR |
| Jobs Agendados | Hangfire |
| Autenticação | JWT Bearer |
| Pagamentos | Asaas (Pix/Boleto/Recorrência) |
| API Gov | Senatran/Detran |
| Testes | xUnit + Moq + FluentAssertions |
| CI | GitHub Actions |

---

## 🚀 Como Rodar Localmente

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 15+](https://www.postgresql.org/)
- [Redis](https://redis.io/) (opcional para desenvolvimento)

### Passos

```bash
# 1. Clone o repositório
git clone https://github.com/mbflowdigital/instrutorpro-api.git
cd instrutorpro-api

# 2. Configure as variáveis de ambiente
cp src/InstrutorPro.API/appsettings.json src/InstrutorPro.API/appsettings.Development.json
# Edite appsettings.Development.json com suas configurações locais

# 3. Restaure as dependências
dotnet restore

# 4. Execute as migrations
dotnet ef database update --project src/InstrutorPro.Infrastructure --startup-project src/InstrutorPro.API

# 5. Execute a API
dotnet run --project src/InstrutorPro.API

# 6. Acesse o Swagger
# http://localhost:5000/swagger
```

### Executar Testes

```bash
# Todos os testes
dotnet test

# Apenas unitários
dotnet test tests/InstrutorPro.UnitTests

# Apenas integração
dotnet test tests/InstrutorPro.IntegrationTests
```

---

## ⚙️ Variáveis de Ambiente

Crie o arquivo `src/InstrutorPro.API/appsettings.Development.json` com:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=instrutorpro;Username=postgres;Password=sua_senha",
    "Redis": "localhost:6379"
  },
  "JwtSettings": {
    "Secret": "sua-chave-secreta-com-pelo-menos-32-caracteres",
    "ExpirationHours": 24,
    "Issuer": "instrutorpro",
    "Audience": "instrutorpro"
  },
  "GovApi": {
    "BaseUrl": "https://api.senatran.gov.br/v1/",
    "ApiKey": "sua-api-key-senatran"
  },
  "Asaas": {
    "BaseUrl": "https://sandbox.asaas.com/api/v3",
    "ApiKey": "sua-api-key-asaas-sandbox"
  }
}
```

> ⚠️ **NUNCA** commite o arquivo `appsettings.Development.json` — ele está no `.gitignore`.

---

## 📁 Estrutura de Pastas Detalhada

```
src/
├── InstrutorPro.Domain/
│   ├── Entities/           # Instrutor, Aluno, Agendamento, Avaliacao, Assinatura
│   ├── Enums/              # StatusInstrutor, StatusAgendamento, CategoriaHabilitacao
│   └── ValueObjects/       # Credenciamento
│
├── InstrutorPro.Application/
│   ├── UseCases/           # Handlers MediatR organizados por feature
│   ├── DTOs/               # Data Transfer Objects de retorno
│   └── Interfaces/         # Contratos para repositórios e serviços externos
│
├── InstrutorPro.Infrastructure/
│   ├── Persistence/        # AppDbContext + Repositórios EF Core
│   ├── GovApi/             # Integração API Senatran/Detran
│   ├── Payment/            # Integração Asaas (Pix/Boleto)
│   └── Jobs/               # Hangfire job de revalidação semanal
│
└── InstrutorPro.API/
    ├── Controllers/        # AuthController, InstrutoresController, AlunosController, AgendamentosController
    ├── Middlewares/        # ExceptionMiddleware global
    ├── Program.cs          # Configuração de serviços e pipeline HTTP
    └── appsettings.json    # Configurações (sem dados sensíveis)
```

---

## 🔄 Fluxo de Validação de Credenciamento

```
Instrutor se cadastra (status: Pendente)
        │
        ▼
Informa CPF + número do credenciamento
        │
        ▼
API chama Senatran/Detran
        │
   ┌────▼────┐
   │Validado?│
   └────┬────┘
   Sim  │  Não
        │         └──→ Status: Pendente (aguarda regularização)
        ▼
Status: Ativo → aparece nas buscas

Job semanal (Hangfire) revalida todos os instrutores ativos.
Se credenciamento revogado → Status: Suspenso (invisível nas buscas).
```

---

## 📜 Licença

Proprietário — © 2026 MBFlow Digital. Todos os direitos reservados.

