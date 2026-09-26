# Management System — Backend

.NET 8 Web API using Clean Architecture, **Entity Framework Core**, and **Azure SQL Server**.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Azure SQL: database **`sql-taskmgmt-karan`** on server **`rg-taskmanagement-prod.database.windows.net`**

## Database connection (local)

Do **not** commit passwords or production connection strings.

### Option A — `appsettings.Local.json` (easy to edit)

1. Copy [`src/ManagementSystem.Api/appsettings.Local.json.example`](src/ManagementSystem.Api/appsettings.Local.json.example) to `appsettings.Local.json` in the same folder.
2. Replace `YOUR_PASSWORD` with your SQL login password.

This file is gitignored (`**/appsettings.Local.json`).

### Option B — User Secrets

From this folder:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=tcp:rg-taskmanagement-prod.database.windows.net,1433;Initial Catalog=sql-taskmgmt-karan;Persist Security Info=False;User ID=sqladmin;Password=YOUR_PASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" --project src/ManagementSystem.Api/ManagementSystem.Api.csproj
```

You can use **both**; `appsettings.Local.json` is loaded after default config and overrides User Secrets when keys conflict.

**Azure firewall:** On logical server `rg-taskmanagement-prod`, add your **current client IP** (Azure Portal → SQL server → Networking → Firewall rules → **Add your client IP address**) and enable **Allow Azure services** for future App Service deployment. If `dotnet ef` or the API fails with **40615** (*IP is not allowed*), update this rule and wait up to 5 minutes, then retry.

## Build and run

```powershell
dotnet build ManagementSystem.sln
dotnet run --project src/ManagementSystem.Api/ManagementSystem.Api.csproj
```

In **Development**, the app applies pending migrations and seeds demo users on startup (if the database is reachable).

- Swagger: `http://localhost:5034/swagger`
- Health: `GET /api/health`
- Database check: `GET /api/health/db`

## EF Core migrations

Tool manifest: [`.config/dotnet-tools.json`](.config/dotnet-tools.json)

Stop any running API first (`Ctrl+C` on `dotnet run`) — otherwise `dotnet build` / `dotnet ef database update` may fail with **MSB3027** (DLL file locked by `ManagementSystem.Api`).

```powershell
dotnet tool restore
dotnet ef migrations add MigrationName --project src/ManagementSystem.Infrastructure --startup-project src/ManagementSystem.Api
dotnet ef database update --project src/ManagementSystem.Infrastructure --startup-project src/ManagementSystem.Api
```

## Demo users (after seed)

| Role    | Email            | Password     |
|---------|------------------|--------------|
| Admin   | admin@demo.com   | `Admin@123`  |
| Manager | manager@demo.com | `Manager@123`|
| User    | user@demo.com    | `User@123`   |

Use only for local/demo environments. Login API is Phase 1.

## Production (Azure App Service)

Set application setting:

`ConnectionStrings__DefaultConnection` = ADO.NET connection string (same shape as above).

## Solution layout

```text
src/
├── ManagementSystem.Domain/         # Entities, enums
├── ManagementSystem.Application/    # Use cases (Phase 1+)
├── ManagementSystem.Infrastructure/ # EF Core, persistence, seed
└── ManagementSystem.Api/            # HTTP API, Swagger
```
