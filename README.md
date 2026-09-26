# Management System

Monorepo for the Management System application: .NET backend (Clean Architecture) and a future frontend.

**Implementation roadmap:** [docs/PROJECT_PLAN.md](docs/PROJECT_PLAN.md) (Team Task Management System — assignment requirements, architecture, phases, and deliverables).

## Repository layout

```text
Management-System/
├── docs/
│   └── PROJECT_PLAN.md            # Full project plan (assessment)
├── Management-System-Backend/     # .NET 8 API (Clean Architecture)
│   ├── ManagementSystem.sln
│   └── src/
│       ├── ManagementSystem.Domain/
│       ├── ManagementSystem.Application/
│       ├── ManagementSystem.Infrastructure/
│       └── ManagementSystem.Api/
└── Management-System-Frontend/    # UI (placeholder — see folder README)
```

## Backend

See **[Management-System-Backend/README.md](Management-System-Backend/README.md)** for **Azure SQL** connection (User Secrets), EF migrations, demo seed users, and health endpoints.

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Build and run

```powershell
cd Management-System-Backend
dotnet build ManagementSystem.sln
dotnet run --project src/ManagementSystem.Api/ManagementSystem.Api.csproj
```

Open Swagger in Development: `http://localhost:5034/swagger`

In Visual Studio, open `Management-System-Backend/ManagementSystem.sln` and set **ManagementSystem.Api** as the startup project.

## Frontend

See [Management-System-Frontend/README.md](Management-System-Frontend/README.md).
