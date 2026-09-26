# Management System — Backend

.NET 8 Web API using Clean Architecture, **Entity Framework Core**, and **Azure SQL Server**.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Azure SQL: database **`sql-taskmgmt-karan`** on server **`rg-taskmanagement-prod.database.windows.net`**

## Database connection (local)

Do **not** commit passwords or production connection strings.

### Option A — `appsettings.Local.json` (easy to edit)

1. Copy [`src/ManagementSystem.Api/appsettings.Local.json.example`](src/ManagementSystem.Api/appsettings.Local.json.example) to `appsettings.Local.json` in the same folder.
2. Replace `YOUR_PASSWORD` with your SQL login password and set `Jwt:Key` to a secret of **at least 32 characters** (do not commit real keys).

This file is gitignored (`**/appsettings.Local.json`).

### JWT signing key

The API requires `Jwt:Key` (min 32 characters). Issuer, audience, and token lifetime are in [`appsettings.json`](src/ManagementSystem.Api/appsettings.json).

**User Secrets** (from this folder):

```powershell
dotnet user-secrets set "Jwt:Key" "YOUR_LOCAL_JWT_SIGNING_KEY_AT_LEAST_32_CHARS" --project src/ManagementSystem.Api/ManagementSystem.Api.csproj
```

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

### Auth (Phase 1)

| Method | Endpoint | Notes |
|--------|----------|--------|
| POST | `/api/auth/login` | Returns JWT + user profile |
| POST | `/api/auth/register` | Creates account with role **User** |

**Swagger:** `POST /api/auth/login` with a demo user → copy `accessToken` → **Authorize** with `Bearer {token}` for future protected APIs.

### Teams & users (Phase 2)

| Method | Endpoint | Roles | Notes |
|--------|----------|-------|--------|
| GET | `/api/teams` | Admin, Manager | Admin: all teams; Manager: teams they belong to |
| POST | `/api/teams` | Admin | Create team |
| PUT | `/api/teams/{id}` | Admin | Update name/description |
| POST | `/api/teams/{id}/members` | Admin, Manager | Body: `{ "userId": "..." }`; Manager only on their teams |
| DELETE | `/api/teams/{id}/members/{userId}` | Admin, Manager | Same scope as add member |
| GET | `/api/users` | Admin, Manager | Scoped user list for assignment dropdowns; optional `?teamId=` |

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

Use only for local/demo environments.

## Production (Azure App Service)

Set application settings:

- `ConnectionStrings__DefaultConnection` = ADO.NET connection string (same shape as above).
- `Jwt__Key` = signing key (min 32 characters; store as a secret).

## Solution layout

```text
src/
├── ManagementSystem.Domain/         # Entities, enums
├── ManagementSystem.Application/    # DTOs, Models, Common, Services, Abstractions
├── ManagementSystem.Infrastructure/ # EF Core, persistence, seed
└── ManagementSystem.Api/            # HTTP API, Swagger
```
