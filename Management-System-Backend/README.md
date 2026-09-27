# Management System — Backend

.NET 8 Web API using Clean Architecture, **Entity Framework Core**, and **Azure SQL Server**.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Azure SQL: database **`sql-taskmgmt-karan`** on server **`rg-taskmanagement-prod.database.windows.net`**

## Database connection (local)

Do **not** commit passwords or production connection strings.

### `appsettings.Local.json` (recommended)

1. Copy [`src/ManagementSystem.Api/appsettings.Local.json.example`](src/ManagementSystem.Api/appsettings.Local.json.example) to `appsettings.Local.json` in the same folder (gitignored).
2. Set `Jwt:Key` to at least **32 characters**.
3. Keep both connection strings in the file; switch with one setting:

| `Database:ConnectionProfile` | Uses |
|------------------------------|------|
| **`Local`** (default) | `ConnectionStrings:Local` — LocalDB `(localdb)\mssqllocaldb`, database **`ManagementSystemDb`** |
| **`Azure`** | `ConnectionStrings:Azure` — replace `YOUR_PASSWORD`; requires Azure SQL firewall |

**Local first run** (API stopped):

```powershell
dotnet tool restore
dotnet ef database update --project src/ManagementSystem.Infrastructure --startup-project src/ManagementSystem.Api
```

In **Development**, `dotnet run` applies migrations and seeds a full demo dataset when there are fewer than **20 teams** or **20 tasks** (28 users, 20 teams, 25+ rows in team members / tasks / comments / notifications). Core logins: `admin@demo.com` / `Admin@123`, `manager@demo.com` / `Manager@123`, `user@demo.com` / `User@123`; extra users use `User@123` or `Manager@123`.

**SSMS:** server `(localdb)\MSSQLLocalDB` → database `ManagementSystemDb` (created by migration).

### JWT signing key

The API requires `Jwt:Key` (min 32 characters). Issuer, audience, and token lifetime are in [`appsettings.json`](src/ManagementSystem.Api/appsettings.json).

Optional User Secrets for `Jwt:Key` only; `appsettings.Local.json` is loaded **after** User Secrets and wins on conflicts.

**Azure firewall (when `ConnectionProfile` is `Azure`):** On logical server `rg-taskmanagement-prod`, add your **current client IP** (Azure Portal → SQL server → Networking → Firewall rules). Error **40615** = IP not allowed; **40613** = database unavailable/paused.

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

### Tasks

| Method | Path | Roles | Notes |
|--------|------|-------|-------|
| GET | `/api/tasks` | Authenticated | List scoped by role; filters bind from query (`ListTasksQueryRequest`: `status`, `priority`, `assigneeId`, `teamId`, `dueBefore`, `dueAfter`) |
| GET | `/api/tasks/{id}` | Authenticated | Task detail |
| POST | `/api/tasks` | Admin, Manager | Create; Manager requires `teamId` and team membership |
| PUT | `/api/tasks/{id}` | Admin, Manager | Update fields / assignee |
| PATCH | `/api/tasks/{id}/status` | Authenticated | Status update; User only on own assignments |
| DELETE | `/api/tasks/{id}` | Admin, Manager | Delete task |

Task detail (`GET /api/tasks/{id}`) includes a `comments` array (oldest first).

### Task comments

| Method | Path | Roles | Notes |
|--------|------|-------|--------|
| GET | `/api/tasks/{id}/comments` | Authenticated | Same task visibility as task detail |
| POST | `/api/tasks/{id}/comments` | Authenticated | Body: `{ "body": "..." }`; author is the signed-in user |

Creating or reassigning a task notifies the assignee (unless they are the actor). Changing task status notifies the assignee and creator (excluding the actor).

### Notifications (Phase 3b)

| Method | Path | Roles | Notes |
|--------|------|-------|--------|
| GET | `/api/notifications` | Authenticated | Current user’s notifications, newest first; optional `?unreadOnly=true` |
| PATCH | `/api/notifications/{id}/read` | Authenticated | Mark one notification read (403 if not owned) |

### Dashboard (Phase 4)

| Method | Path | Roles | Notes |
|--------|------|-------|--------|
| GET | `/api/dashboard/summary` | Authenticated | Task counts by status in role scope; optional filters: `priority`, `assigneeId`, `teamId`, `dueBefore`, `dueAfter` (status filter ignored so all three buckets are returned). Includes `unreadNotificationCount`. Task lists use `GET /api/tasks`. |

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
