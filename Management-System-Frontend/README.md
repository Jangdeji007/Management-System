# Management System — Frontend (TaskFlow)

React + TypeScript + Vite SPA for the Team Task Management System. Uses **Axios** to call the .NET API.

UI/UX plan: [Team_Task_Management_System_UI_Project_Plan.md](./Team_Task_Management_System_UI_Project_Plan.md)

## Prerequisites

- [Node.js](https://nodejs.org/) 20+ (22 recommended)
- Backend API running locally (see [Management-System-Backend/README.md](../Management-System-Backend/README.md))

## Setup

```powershell
cd Management-System-Frontend
npm install
```

Copy environment variables if needed:

```powershell
copy .env.example .env.development
```

Default API URL: `http://localhost:5034` (matches the backend **http** launch profile).

## Run

```powershell
npm run dev
```

Open [http://localhost:5173](http://localhost:5173).

In another terminal, start the API:

```powershell
cd Management-System-Backend
dotnet run --project src/ManagementSystem.Api/ManagementSystem.Api.csproj
```

## Demo accounts

| Role    | Email            | Password     |
|---------|------------------|--------------|
| Admin   | admin@demo.com   | Admin@123    |
| Manager | manager@demo.com | Manager@123  |
| User    | user@demo.com    | User@123     |

## Features

- JWT login and registration with protected routes
- Role-based top navigation (Admin / Manager / User)
- Dashboard: welcome, stat cards, status chart, recent tasks, upcoming deadlines, filters
- Tasks: search (client-side), API filters, table/cards, create-task modal, detail, status, comments
- Teams: card list, team detail with members and team tasks
- Admin users list (read-only)
- Notifications bell + full notifications page
- Settings: read-only profile
- Toasts (Sonner), Lucide icons, Inter font

## Build

```powershell
npm run build
npm run preview
```
