# Team Task Management System – UI/UX Project Plan

## 1. Project Overview

This document defines the UI/UX plan for the **Team Task Management System**.

The application is a role-based task management system with three primary roles:

- **Admin** – manages teams and assigns tasks to managers and users.
- **Manager** – creates and assigns tasks to team members.
- **User** – views and manages assigned tasks.

The UI will follow a **modern SaaS dashboard** style with a clean, responsive and professional design.

The assessment specifically requires a clean, responsive UI with API integration and appropriate error handling.

---

# 2. UI Design Goals

The main goals of the UI are:

- Clean and professional appearance
- Simple navigation
- Easy task management
- Clear task status visibility
- Role-based navigation
- Responsive design for desktop and mobile
- Consistent colors, spacing and typography
- Good readability and accessibility
- Production-oriented SaaS dashboard experience

---

# 3. Recommended Design Theme

## Theme

**Modern SaaS Dashboard**

The overall interface should look similar to a professional project/task management application rather than a basic CRUD application.

## Color Palette

| UI Element | Color |
|---|---|
| Primary | `#2563EB` |
| Primary Dark | `#1D4ED8` |
| Background | `#F8FAFC` |
| Sidebar | `#0F172A` |
| Card | `#FFFFFF` |
| Main Text | `#0F172A` |
| Secondary Text | `#64748B` |
| Border | `#E2E8F0` |
| To Do | `#64748B` |
| In Progress | `#F59E0B` |
| Done | `#10B981` |
| Error/Delete | `#EF4444` |

### Color Usage Rule

Use blue as the main brand/action color.

Do not use too many colors throughout the application.

Use status colors only where they provide useful information:

- Gray → To Do
- Orange → In Progress
- Green → Done
- Red → Error/Delete

---

# 4. Typography

Recommended font:

**Inter**

Typography should be simple and readable.

Suggested hierarchy:

- Page Heading: 24–28px, bold
- Section Heading: 18–20px, semi-bold
- Body Text: 14–16px
- Secondary Text: 12–14px
- Button Text: 14px, medium/semi-bold

---

# 5. Application Layout

The authenticated layout uses a **sticky top navbar** plus main content (no persistent sidebar).

```text
------------------------------------------------------------
| TaskFlow | Dashboard Tasks ...     | 🔔  Karan ▼        |
------------------------------------------------------------
|                      Page Content                          |
------------------------------------------------------------
```

- **Left:** brand **TaskFlow** and role-based navigation links.
- **Right:** notification bell (with unread badge), user name/role, profile menu (Settings, Log out).
- **Comments** are not a top-level route; they appear on **Task Details** only.

On **mobile**, primary nav links move into a **hamburger drawer**; bell and profile stay in the header.

The active nav item uses primary blue (`#2563EB` / Tailwind `blue-600`).

---

# 6. Top Navbar

The top navbar should contain:

- Brand: **TaskFlow**
- Role-based navigation links (see Section 14)
- Notification icon (links to quick panel and full `/notifications` page)
- User name and role
- Profile dropdown (Settings, Log out)

Example:

```text
TaskFlow  Dashboard  Tasks  Teams              🔔    👤 Karan ▼
```

---

# 7. Dashboard UI

The dashboard should provide a quick overview of task activity.

## Dashboard Header

```text
Dashboard
Welcome back, Karan
```

Unread notification count appears on the **navbar bell badge**, not as a fifth dashboard stat card.

## Statistics Cards

Display four main cards:

```text
+----------------+ +----------------+ +----------------+ +----------------+
| Total Tasks    | | To Do          | | In Progress    | | Completed      |
|                | |                | |                | |                |
|      24        | |       8        | |       10       | |       6        |
+----------------+ +----------------+ +----------------+ +----------------+
```

Cards should have:

- White background
- 10–12px border radius
- Light shadow
- Clear number
- Small supporting text where applicable

## Dashboard Content

Recommended sections:

1. Task status overview
2. Recent tasks
3. Upcoming deadlines
4. Task statistics/chart

Example:

```text
+--------------------------------+ +---------------------------+
| Task Overview                  | | Recent Tasks              |
|                                | |                           |
|        Status Chart            | | • Login API               |
|                                | | • UI Design               |
|                                | | • Database Setup          |
|                                | | • Testing                 |
+--------------------------------+ +---------------------------+
```

---

# 8. Tasks Page

The Tasks page is the main feature of the application and should receive the most UI attention.

## Page Header

```text
Tasks

Search tasks...       Status ▼   Priority ▼   Deadline ▼

                                      + Create Task
```

## Task Table

Recommended columns:

| Task | Assigned To | Priority | Status | Due Date | Action |
|---|---|---|---|---|---|
| Login API | Rahul | High | In Progress | 28 Sep | View |
| UI Design | Karan | Medium | To Do | 30 Sep | View |
| Database | Amit | High | Done | 25 Sep | View |

## Status Badges

Use small badges instead of large colored blocks.

Examples:

```text
● To Do
● In Progress
● Done
```

Suggested colors:

- To Do → Gray
- In Progress → Orange
- Done → Green

---

# 9. Task Filters

The dashboard/task page should support filtering by:

- Deadline
- Status
- Priority

The assessment specifically requires filtering by these task attributes.

Recommended UI:

```text
[ Search... ]

[ Status ▼ ] [ Priority ▼ ] [ Due after ] [ Due before ] [ Clear filters ]
```

**Search:** client-side filter on task title (and assignee name) after the API returns results until a backend `search` query exists.

**Deadline:** implemented as **Due after** and **Due before** date inputs matching `GET /api/tasks` query params.

---

# 10. Create Task UI

Use a modal for creating a task.

Example:

```text
+--------------------------------------+
| Create New Task                  X   |
+--------------------------------------+
|                                      |
| Task Title                           |
| [ Enter task title................ ] |
|                                      |
| Description                          |
| [................................. ] |
| [................................. ] |
|                                      |
| Assign To                            |
| [ Select team member            ▼ ] |
|                                      |
| Priority                             |
| [ Medium                         ▼ ] |
|                                      |
| Due Date                             |
| [ 30 Sep 2026                    ]  |
|                                      |
|          [ Cancel ] [ Create Task ]  |
+--------------------------------------+
```

Fields:

- Task Title
- Description
- Assign To
- Priority
- Due Date

---

# 11. Task Details Page

Clicking a task should open a detailed task view.

Recommended information:

```text
← Back to Tasks

Implement Login API

Status       In Progress
Priority     High
Assigned To  Karan
Due Date     30 Sep 2026

Description
------------------------------------------------
Implement JWT based authentication API...

Comments
------------------------------------------------

Rahul
Looks good. JWT implementation completed.

Karan
I'll complete refresh token tomorrow.

[ Write a comment......................... ] [Send]
```

The comments section should allow users to collaborate on tasks.

---

# 12. Team Management UI

Teams should be displayed as cards.

Example:

```text
Teams                                      + Create Team

+------------------------------------------------------+
| Development Team                                     |
|                                                      |
| 👤 Karan   👤 Rahul   👤 Amit   +2 more             |
|                                                      |
| 5 Members                         View Team →         |
+------------------------------------------------------+

+------------------------------------------------------+
| QA Team                                              |
|                                                      |
| 👤 Priya   👤 Raj   👤 Ankit                        |
|                                                      |
| 3 Members                         View Team →         |
+------------------------------------------------------+
```

Team details can show:

- Team name
- Team members
- Manager
- Number of members
- Assigned tasks

---

# 13. Notifications UI

Notifications should be accessible from the top-right notification icon.

Example:

```text
Notifications

+------------------------------------------+
| 🔵 Rahul assigned you a new task         |
|    5 minutes ago                         |
+------------------------------------------+

+------------------------------------------+
| 🟢 Login API marked as Done              |
|    20 minutes ago                        |
+------------------------------------------+

+------------------------------------------+
| 🟡 Task deadline tomorrow                |
|    1 hour ago                            |
+------------------------------------------+
```

Notifications should cover important task events such as:

- Task assignment
- Task status update

---

# 14. Role-Based UI

The navigation should change based on the logged-in user's role.

## Admin

| Label | Route |
|---|---|
| Dashboard | `/` |
| Users | `/users` |
| Teams | `/teams` |
| Tasks | `/tasks` |
| Notifications | bell + `/notifications` |
| Settings | `/settings` |

Admin can manage teams and assign tasks (via Create Task on Tasks).

## Manager

| Label | Route |
|---|---|
| Dashboard | `/` |
| My Team | `/teams` |
| Tasks | `/tasks` |
| Create Task | button on Tasks (opens modal) |
| Notifications | bell + `/notifications` |
| Settings | `/settings` |

Manager can create and assign tasks to team members.

## User

| Label | Route |
|---|---|
| Dashboard | `/` |
| My Tasks | `/tasks` (same route, role-specific label) |
| Notifications | bell + `/notifications` |
| Settings | `/settings` |

Users should not see administrative navigation options (no Users, Teams, or Create Task).

---

# 15. Login Page

The login page should be minimal and professional.

```text
                  TaskFlow
           Team Task Management

       +--------------------------+
       | Email                    |
       | [ john@example.com      ]|
       |                          |
       | Password                 |
       | [ ***************      ] |
       |                          |
       |        [ Login ]         |
       |                          |
       | Create account → Register|
       +--------------------------+
```

**Forgot password** is out of scope (no API).

Recommended design:

- Light gray background
- White login card
- Blue primary button
- Rounded input fields
- Clear validation messages

---

# 16. Buttons

Use consistent button styles.

## Primary Button

Blue:

```text
+------------------+
|  + Create Task   |
+------------------+
```

## Secondary Button

White/light gray:

```text
+------------------+
|      Cancel      |
+------------------+
```

## Danger Button

Red:

```text
Delete
```

Buttons should use approximately:

- Border radius: 8px
- Height: 38–42px
- Horizontal padding: 14–18px

---

# 17. Cards and Spacing

Recommended:

- Border radius: 10–12px
- Card background: White
- Border: `#E2E8F0`
- Light shadow
- Consistent internal padding
- Use a spacing system such as 4px/8px multiples

Avoid crowded screens.

---

# 18. Mobile Responsive Design

The application should be responsive.

## Desktop

```text
Top Navbar + Content
```

## Mobile

Primary navigation collapses into a hamburger menu.

```text
☰  TaskFlow                         🔔 👤
```

Dashboard cards should stack vertically.

Task tables can become task cards:

```text
+--------------------------+
| Login API                |
|                          |
| 🔴 High                  |
| 🟡 In Progress           |
| 👤 Karan                 |
| 📅 30 Sep                |
+--------------------------+
```

---

# 19. Error and Validation UI

The UI should provide clear feedback for API errors and validation errors.

Examples:

### Success

```text
✓ Task created successfully
```

### Error

```text
✕ Unable to create task. Please try again.
```

### Validation

```text
Task title is required.
```

### Unauthorized

```text
You don't have permission to perform this action.
```

Use toast notifications for short-lived messages.

---

# 20. Loading States

Avoid showing a blank screen while API requests are running.

Use:

- Spinner
- Skeleton loading
- Disabled submit button

Example:

```text
Creating task...
[ Loading... ]
```

---

# 21. Empty States

If there are no tasks:

```text
              📋

          No tasks found

There are no tasks matching your filters.

             [ Clear Filters ]
```

For empty teams:

```text
No teams created yet.

             + Create Team
```

---

# 22. Icons

Use one consistent icon library throughout the application.

Recommended:

**Lucide Icons**

Suggested icons:

- Dashboard → LayoutDashboard
- Tasks → ClipboardList
- Teams → Users
- Notifications → Bell
- Settings → Settings
- Logout → LogOut
- Search → Search
- Add → Plus
- Calendar → Calendar
- Edit → Pencil
- Delete → Trash2

Do not mix multiple icon styles.

---

# 23. Suggested Application Pages

The frontend should contain approximately these pages:

```text
Authentication
├── Login
└── Register

Dashboard
└── Dashboard

Tasks
├── Task List
├── Create Task
└── Task Details

Teams
├── Team List
├── Create Team
└── Team Details

Notifications
└── Notification List

Profile/Settings
└── User Settings
```

Role-based access should determine which pages/features are available.

---

# 24. Recommended Final UI Structure

```text
TaskFlow
│
├── Authentication
│   ├── Login
│   └── Register
│
├── Dashboard
│   ├── Statistics Cards
│   ├── Task Overview
│   ├── Recent Tasks
│   └── Upcoming Deadlines
│
├── Tasks
│   ├── Search
│   ├── Filters
│   ├── Task Table
│   ├── Create Task
│   └── Task Details
│
├── Teams
│   ├── Team Cards
│   ├── Members
│   └── Team Details
│
├── Notifications
│   └── Notification List
│
└── Settings
    └── Profile
```

---

# 25. Final Design Recommendation

Use the following design system consistently:

```text
Application Style : Modern SaaS
Primary Color     : #2563EB
Sidebar           : #0F172A
Background        : #F8FAFC
Cards             : #FFFFFF
Font              : Inter
Border Radius     : 10–12px
Primary Button    : Blue
To Do             : Gray
In Progress       : Orange
Done              : Green
Error/Delete      : Red
Icons             : Lucide
Layout            : Top Navbar + Content (mobile hamburger nav)
Task View         : Table + Filters + client-side search
Task Details      : Details + Comments
Mobile            : Responsive / hamburger nav + task cards
Toasts            : Sonner for success/error feedback
```

The goal is to make the application look like a **real production-oriented task management SaaS product**, while keeping the UI simple, clean and easy to navigate.

---

# 26. Implementation Tech Stack

| Layer | Choice |
|---|---|
| UI | React 19, TypeScript |
| Build | Vite |
| Styling | Tailwind CSS 4 |
| Routing | React Router 7 |
| HTTP | Axios (`VITE_API_URL`, default `http://localhost:5034`) |
| Icons | Lucide React |
| Toasts | Sonner |

---

# 27. Backend API Map (UI)

| Feature | Method | Path |
|---|---|---|
| Login | POST | `/api/auth/login` |
| Register | POST | `/api/auth/register` |
| Current user | GET | `/api/auth/me` |
| Dashboard summary | GET | `/api/dashboard/summary` |
| List tasks | GET | `/api/tasks` |
| Task detail | GET | `/api/tasks/{id}` |
| Create task | POST | `/api/tasks` |
| Update task / status | PUT | `/api/tasks/{id}` |
| Comments | GET/POST | `/api/tasks/{id}/comments` |
| Teams | GET/POST/PUT | `/api/teams`, `/api/teams/{id}` |
| Team members | POST/DELETE | `/api/teams/{id}/members` |
| Users (Admin/Manager) | GET | `/api/users` |
| Notifications | GET | `/api/notifications` |
| Mark read | PATCH | `/api/notifications/{id}/read` |

---

# 28. Out of Scope (v1)

- Forgot password / email reset
- Profile or password edit (Settings is read-only from `/api/auth/me`)
- Admin user CRUD (Users page is read-only list from `GET /api/users`)
- Server-side task title search (use client-side search until API adds it)

---

# 29. Implementation Baseline

Already implemented before full plan rollout:

- JWT login, protected routes, API client with error handling
- Dashboard summary + task filters (status, priority, due range)
- Task list (table + mobile cards), detail, status update, comments
- Teams page with create/members (Admin/Manager)
- Notification bell dropdown

Delivered in implementation phases: design tokens, role-based nav, dashboard chart/sections, create-task modal, team cards/detail route, register, users/notifications/settings pages, toasts and polish.

---

# 30. Settings (v1)

Settings page shows read-only profile: full name, email, role, account created date from `/api/auth/me`. Editing requires a future profile-update API.
