# 🚀 CodeClash

<p align="center">
  <b>Modern Online Judge & Competitive Programming Platform</b>
</p>

<p align="center">
  Built with ASP.NET Core, Clean Architecture, CQRS, Redis, Docker, and PostgreSQL.
</p>

---

<p align="center">

  <img src="https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" />
  <img src="https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white" />
  <img src="https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white" />
  <img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white" />
  <img src="https://img.shields.io/badge/OpenTelemetry-000000?style=for-the-badge&logo=opentelemetry&logoColor=white" />
  <img src="https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black" />
</p>

---

# 📖 Overview

**CodeClash** is a modern, scalable, and extensible online coding contest platform and problem-solving API built using **ASP.NET Core 9** and **Clean Architecture**.

The platform enables users to:

- 🏆 Participate in programming contests
- 📝 Solve algorithmic problems
- 🚀 Submit code for secure evaluation
- 📊 Track progress and submissions
- 🔐 Manage authentication and authorization

CodeClash focuses on:
- ⚡ Performance
- 🔒 Security
- 🧩 Scalability
- 🏗 Maintainability

---

# ✨ Features

## 🏆 Contest Management
- Create and manage coding contests
- Contest registration system
- Contest scheduling and tracking
- Contest participant management

## 📝 Problem Management
- Add, update, and delete coding problems
- Problem difficulty levels
- Topics and categorization
- Test cases support
- Problem images and descriptions

## ⚡ Online Judge System
- Secure code execution using Docker
- Real-time judging and feedback
- Execution time tracking
- Memory usage tracking
- Multi-language support ready

## 🔐 Authentication & Authorization
- JWT Authentication
- ASP.NET Identity integration
- Role-Based Authorization
- Refresh Tokens
- Email Confirmation

## 📊 Monitoring & Performance
- Redis response caching
- OpenTelemetry tracing
- Structured logging with Serilog + Seq
- Stateless scalable API

## 🧩 Architecture & Design
- Clean Architecture
- CQRS Pattern
- MediatR
- Repository Pattern
- Dependency Injection
- Unit of Work

---

# 🛠 Tech Stack

| Category | Technologies |
|----------|--------------|
| **Backend** | ASP.NET Core 9, C# |
| **Architecture** | Clean Architecture, CQRS, MediatR |
| **Database** | PostgreSQL, Entity Framework Core |
| **Caching** | Redis, StackExchange.Redis |
| **Authentication** | JWT, ASP.NET Identity |
| **Containerization** | Docker |
| **Observability** | OpenTelemetry, Serilog, Seq |
| **Search** | ElasticSearch |
| **Documentation** | Swagger / OpenAPI |
| **Testing** | xUnit, Moq |

---

# 📂 Project Structure

```bash
src/
│
├── CodeClash.API/               # API Endpoints & Configuration
├── CodeClash.Application/       # CQRS, DTOs, Business Logic
├── CodeClash.Domain/            # Entities & Domain Rules
├── CodeClash.Infrastructure/    # Database, Redis, Docker, Services
│
tests/
├── CodeClash.UnitTests/
└── CodeClash.IntegrationTests/
```

---

# 🗄 Database Design

## Main Entities

- 👤 Users
- 🏆 Contests
- 📝 Problems
- 📨 Submits
- 🧪 Testcases
- 🏷 Topics
- 🔗 UserContests
- 📰 Blogs

## Relationships

- A `User` can create many `Problems`, `Contests`, and `Blogs`
- A `Contest` contains multiple `Problems`
- A `Problem` contains multiple `Testcases`
- A `UserContest` links users with contests
- A `Submit` belongs to a `User` and a `Problem`

---

# 📚 API Documentation

Swagger documentation is available at:

```bash
http://localhost:8080/swagger
```

---

# 🔥 Sample API Endpoints

## 🔐 Authentication

### Register

```http
POST /auth/register
```

```json
{
  "email": "user@example.com",
  "password": "P@ssw0rd!",
  "name": "Jane Doe"
}
```

### Login

```http
POST /auth/login
```

```json
{
  "email": "user@example.com",
  "password": "P@ssw0rd!"
}
```

---

## 📝 Problems

```http
GET /problems?difficulty=Easy&pageNumber=1&pageSize=10
Authorization: Bearer {token}
```

---

## 🏆 Contests

```http
POST /contests
Authorization: Bearer {token}
```

```json
{
  "name": "Spring Challenge",
  "startDate": "2026-06-01T10:00:00Z",
  "endDate": "2026-06-01T13:00:00Z"
}
```

---

## ⚡ Submissions

```http
POST /submits
Authorization: Bearer {token}
```

```json
{
  "problemId": "GUID",
  "code": "...",
  "language": "CSharp"
}
```

---

# 🔐 Authentication Flow

```text
Register
   ↓
Email Confirmation
   ↓
Login
   ↓
Receive JWT + Refresh Token
   ↓
Access Protected APIs
```

---

# ⚙ Environment Variables

Configure these inside:

```bash
appsettings.Development.json
```

## Database

```json
"ConnectionStrings": {
  "Database": "",
  "Redis": ""
}
```

## JWT

```json
"Jwt": {
  "Key": "",
  "Issuer": "",
  "Audience": ""
}
```

---

# 🚀 Installation Guide

## 1️⃣ Clone Repository

```bash
git clone https://github.com/SeifMohmmed/CodeClash.git
cd CodeClash
```

---

## 2️⃣ Configure Environment Variables

Update:

```bash
appsettings.Development.json
```

---

## 3️⃣ Run Using Docker (Recommended)

```bash
docker-compose up --build
```

---

## 4️⃣ Run Locally

### Requirements
- .NET 9 SDK
- PostgreSQL
- Redis
- Docker

### Run

```bash
dotnet build
dotnet run --project src/CodeClash.API
```

---

# 🐳 Docker Support

## Included Services

- 🌐 ASP.NET Core API
- 🐘 PostgreSQL
- ⚡ Redis
- 📊 Seq Logging
- 🔍 ElasticSearch

---

# ⚡ Redis / Caching

- ResponseCacheService caches API responses in Redis
- Contest problems cached during active contests
- Reduces database load
- Improves API performance

---

# 📊 Observability

Integrated with:
- OpenTelemetry
- Serilog
- Seq

Supports:
- Distributed tracing
- Structured logging
- Monitoring & diagnostics

---

# 🧪 Testing

## Testing Stack

- xUnit
- Moq

## Run Tests

```bash
dotnet test
```

---

# 🔄 CI/CD

GitHub Actions can be used for:
- Build automation
- Running tests
- Docker image publishing
- Deployment pipelines

---

# 🔒 Security Considerations

- JWT Authentication
- Role-Based Access
- HTTPS Enforcement
- Input Validation
- Docker Isolation
- Email Verification

---

# 📈 Scalability Notes

- Stateless API design
- Redis caching
- Dockerized services
- Horizontal scaling ready
- CQRS separation

---

# 🚀 Future Improvements

- SignalR real-time contest updates
- User leaderboards
- Admin dashboard
- More programming languages
- Rate limiting
- Kubernetes deployment

---

# 📸 Screenshots

| Contest List | Problem Details | Submission Result |
|--------------|----------------|------------------|
| ![](docs/screenshots/contest-list.png) | ![](docs/screenshots/problem-details.png) | ![](docs/screenshots/submission-result.png) |

---
