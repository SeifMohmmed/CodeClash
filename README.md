# 🚀 CodeClash

<p align="center">
  <b>Modern Online Judge & Competitive Programming Platform</b>
</p>

<p align="center">
  Built with ASP.NET Core, Clean Architecture, CQRS, Redis, Docker, and PostgreSQL.
</p>

<p align="center">

  <img src="https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" />
  <img src="https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white" />
  <img src="https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white" />
  <img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white" />
  <img src="https://img.shields.io/badge/OpenTelemetry-000000?style=for-the-badge&logo=opentelemetry&logoColor=white" />
  <img src="https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black" />
</p>

# 📖 Overview

**CodeClash** is a scalable and extensible online coding contest platform and problem-solving API built using **ASP.NET Core 9** and **Clean Architecture**.

The platform allows users to:

- Participate in coding contests
- Solve algorithmic problems
- Submit and evaluate code securely
- Track progress and submissions
- Manage contests and problems

CodeClash focuses on:
- ⚡ Performance
- 🔒 Security
- 🧩 Scalability
- 🏗 Maintainability

---

# ✨ Features

## 🏆 Contest Management
- Create and manage programming contests
- Contest registration system
- Contest scheduling and tracking
- Contest participant management

## 📝 Problem Management
- CRUD operations for coding problems
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

## 🚀 Performance & Scalability
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

# 🚀 Installation

```bash
git clone https://github.com/SeifMohmmed/CodeClash.git
cd CodeClash
docker-compose up --build
```

---
