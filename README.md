# 🚀 CodeClash

<p align="center">
  <b>Scalable Online Judge & Competitive Programming Platform</b>
</p>

<p align="center">

  <img src="https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" />
  <img src="https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white" />
  <img src="https://img.shields.io/badge/ElasticSearch-005571?style=for-the-badge&logo=elasticsearch&logoColor=white" />
  <img src="https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white" />
  <img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white" />
  <img src="https://img.shields.io/badge/OpenTelemetry-000000?style=for-the-badge&logo=opentelemetry&logoColor=white" />
  <img src="https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black" />
</p>

---

# 📖 Overview

**CodeClash is a scalable online judge and competitive programming platform built to support coding contests, algorithmic problem solving, and secure code evaluation**.

**The platform provides contest management, Docker-based isolated execution, background processing, real-time updates, observability, and distributed caching for high-load scenarios.**

---

### 🔭 System Architecture
<p align="center">
  <img src="https://github.com/SeifMohmmed/CodeClash/blob/3db8b241104cdfcf5c17165579ed61e992a4c45a/System-Architecture.png"/>
</p>

## 🧠 Key Engineering Decisions

- **Clean Architecture + CQRS** — enforces a one-way dependency flow (API → Application → Domain), keeping business logic independent of EF Core, Redis, or Docker.
- **Dual DbContext for Identity** — separating `ApplicationIdentityDbContext` from `ApplicationDbContext` avoids coupling domain entities to ASP.NET Identity's schema, while `IdentityId` provides the link.
- **Redis caching for contest data** — live standings and problem sets are cached during active contests, computed via EF LINQ aggregation and Redis sorted sets, with pagination to reduce database load under concurrent traffic.
- **Hangfire for background jobs** — used for post-contest processing (e.g., rank-up calculations) that shouldn't block request/response cycles.
- **SignalR for real-time features** — dedicated hubs isolate real-time concerns from REST endpoints.
- **Docker-isolated judging** — code execution runs in sandboxed containers to prevent untrusted submissions from affecting the host.

---

# ✨ Features

- 🏆 **Contests** — creation, scheduling, registration, participant management, live standings.
- 📝 **Problems** — CRUD operations, Problem difficulty levels, topics/categorization, test cases, image/description support.
- ⚡ **Online Judge** — Docker-isolated execution, real-time judging, execution time & memory tracking, Multi-language support ready.
- 🔐 **Authentication & Authorization** — JWT + refresh tokens, ASP.NET Identity, role-based authorization, email confirmation
- 📡 **Real-time** — SignalR hubs (e.g., contest editor, video chat)
- 📊 **Observability** — OpenTelemetry tracing, structured logging via Serilog + Seq
- 🧩 **Architecture & Design** — - Clean Architecture, CQRS Pattern, MediatR, Repository Pattern, Unit of Work.

---
## 🛠 Tech Stack

| Category | Technologies |
|---|---|
| Backend | ASP.NET Core 9, C# |
| Architecture | Clean Architecture, CQRS, MediatR, Repository + Unit of Work |
| Database | PostgreSQL, EF Core |
| Caching | Redis (StackExchange.Redis) |
| Real-time | SignalR |
| Background Jobs | Hangfire |
| Auth | JWT, ASP.NET Identity, Refresh Tokens, Email Confirmation |
| Containerization | Docker |
| Observability | OpenTelemetry, Serilog, Seq |
| Search | ElasticSearch |
| Testing | xUnit, Moq |
| Docs | Swagger / OpenAPI |

---

# 📂 Project Structure

```bash
src/
├── CodeClash.API/             # Controllers, middleware, Swagger config, SignalR hub registration
├── CodeClash.Application/     # CQRS commands/queries, MediatR handlers, validators, DTOs
├── CodeClash.Domain/          # Entities, enums, domain rules — framework-agnostic
├── CodeClash.Infrastructure/  # EF Core configs, Redis, Docker judge runner, Identity, Hangfire jobs

tests/
├── CodeClash.UnitTests/        # Handler and domain logic tests
└── CodeClash.IntegrationTests/ # API and persistence integration tests
```

---

# 🗄 Domain Model

<p align="center">
  <img src="https://github.com/SeifMohmmed/CodeClash/blob/3db8b241104cdfcf5c17165579ed61e992a4c45a/Class-Diagram.png"/>
</p>


---

# 🚀 Installation Guide

### Prerequisites
- .NET 9 SDK
- PostgreSQL
- Redis
- Docker

### 🐳 Run with Docker (recommended)

```bash
git clone https://github.com/SeifMohmmed/CodeClash.git
cd CodeClash
docker-compose up --build
```

### ▶️ Run Locally

```bash
dotnet build
dotnet run --project src/CodeClash.API
```

Included Docker services: ASP.NET Core API, PostgreSQL, Redis, Seq, ElasticSearch.

📑 API docs available at `https://localhost:5001/swagger/index.html` once running.

---

## ⚙️ Environment Setup

Configure `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Database": "",
    "Redis": ""
  },
  "Jwt": {
    "Key": "",
    "Issuer": "",
    "Audience": ""
  }
}
```

---

## 🧪 Testing

```bash
dotnet test
```

Stack: xUnit + Moq, covering handler logic (unit) and API/persistence flows (integration).

---
