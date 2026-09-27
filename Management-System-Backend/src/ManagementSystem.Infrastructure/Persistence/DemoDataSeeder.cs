using ManagementSystem.Domain.Entities;
using ManagementSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DomainTaskStatus = ManagementSystem.Domain.Enums.TaskStatus;

namespace ManagementSystem.Infrastructure.Persistence;

public static class DemoDataSeeder
{
    private const int MinTaskCount = 20;

    private const int MinTeamCount = 20;

    private static readonly string DefaultDemoPassword = "User@123";

    public static async Task SeedAsync(
        ApplicationDbContext context,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        var taskCount = await context.TaskItems.CountAsync(cancellationToken);
        var teamCount = await context.Teams.CountAsync(cancellationToken);
        if (taskCount >= MinTaskCount && teamCount >= MinTeamCount)
        {
            logger.LogInformation(
                "Demo dataset already present ({TaskCount} tasks, {TeamCount} teams); seed skipped.",
                taskCount,
                teamCount);
            return;
        }

        var hasher = new PasswordHasher<User>();
        var seedTime = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc);

        var usersByEmail = await EnsureUsersAsync(context, hasher, seedTime, cancellationToken);
        var allUsers = usersByEmail.Values.ToList();
        var admin = usersByEmail["admin@demo.com"];

        var teams = await EnsureTeamsAsync(context, admin, seedTime, cancellationToken);
        await EnsureTeamMembersAsync(context, teams, allUsers, seedTime, cancellationToken);

        if (taskCount < MinTaskCount)
        {
            var tasks = CreateTasks(teams, allUsers, admin, seedTime);
            context.TaskItems.AddRange(tasks);
            await context.SaveChangesAsync(cancellationToken);

            var comments = CreateComments(tasks, allUsers, seedTime);
            context.TaskComments.AddRange(comments);

            var notifications = CreateNotifications(tasks, allUsers, seedTime);
            context.Notifications.AddRange(notifications);

            await context.SaveChangesAsync(cancellationToken);
        }

        var memberCount = await context.TeamMembers.CountAsync(cancellationToken);
        var finalTaskCount = await context.TaskItems.CountAsync(cancellationToken);
        var commentCount = await context.TaskComments.CountAsync(cancellationToken);
        var notificationCount = await context.Notifications.CountAsync(cancellationToken);

        logger.LogInformation(
            "Demo seed complete: {Users} users, {Teams} teams, {Members} team members, {Tasks} tasks, {Comments} comments, {Notifications} notifications.",
            allUsers.Count,
            teams.Count,
            memberCount,
            finalTaskCount,
            commentCount,
            notificationCount);
    }

    private static async Task<Dictionary<string, User>> EnsureUsersAsync(
        ApplicationDbContext context,
        PasswordHasher<User> hasher,
        DateTime seedTime,
        CancellationToken cancellationToken)
    {
        var specs = new List<(string Email, string FullName, UserRole Role, string Password)>
        {
            ("admin@demo.com", "System Admin", UserRole.Admin, "Admin@123"),
            ("manager@demo.com", "Priya Sharma", UserRole.Manager, "Manager@123"),
            ("user@demo.com", "Amit Kumar", UserRole.User, "User@123"),
            ("manager2@demo.com", "Rahul Mehta", UserRole.Manager, "Manager@123"),
            ("manager3@demo.com", "Sneha Patel", UserRole.Manager, "Manager@123"),
            ("manager4@demo.com", "Vikram Singh", UserRole.Manager, "Manager@123"),
            ("manager5@demo.com", "Anita Desai", UserRole.Manager, "Manager@123"),
        };

        for (var i = 2; i <= 22; i++)
        {
            specs.Add((
                $"user{i:00}@demo.com",
                $"Demo User {i:00}",
                UserRole.User,
                DefaultDemoPassword));
        }

        var existing = await context.Users
            .AsNoTracking()
            .ToDictionaryAsync(u => u.Email, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var result = new Dictionary<string, User>(StringComparer.OrdinalIgnoreCase);
        var offset = 0;

        foreach (var (email, fullName, role, password) in specs)
        {
            if (existing.TryGetValue(email, out var found))
            {
                result[email] = found;
                continue;
            }

            var user = CreateUser(email, fullName, role, password, hasher, seedTime.AddMinutes(offset++));
            context.Users.Add(user);
            result[email] = user;
        }

        await context.SaveChangesAsync(cancellationToken);
        return result;
    }

    private static async Task<List<Team>> EnsureTeamsAsync(
        ApplicationDbContext context,
        User admin,
        DateTime seedTime,
        CancellationToken cancellationToken)
    {
        var existing = await context.Teams.ToListAsync(cancellationToken);
        if (existing.Count >= MinTeamCount)
        {
            return existing;
        }

        var teamSpecs = new (string Name, string Description)[]
        {
            ("Platform Engineering", "Core API, auth, and infrastructure."),
            ("Product Design", "UX research, wireframes, and design system."),
            ("Customer Success", "Onboarding, support playbooks, and SLAs."),
            ("DevOps & SRE", "CI/CD, observability, and incident response."),
            ("QA & Release", "Test automation and release certification."),
            ("Data & Analytics", "Reporting pipelines and dashboards."),
            ("Mobile Apps", "iOS and Android client features."),
            ("Security & Compliance", "Audits, SOC2, and access reviews."),
            ("Integrations", "Third-party webhooks and partner APIs."),
            ("Technical Writing", "API docs and internal runbooks."),
            ("Site Reliability", "On-call rotation and postmortems."),
            ("Growth Marketing", "Campaign tooling and experiments."),
            ("Finance Ops", "Billing exports and cost tracking."),
            ("HR Systems", "Employee onboarding workflows."),
            ("Legal Review", "Contract and policy checkpoints."),
            ("Research Lab", "Spikes and proof-of-concept work."),
            ("Support Tier 2", "Escalations from frontline support."),
            ("Infrastructure Cost", "Cloud spend optimization."),
            ("Accessibility", "WCAG audits and fixes."),
            ("Release Management", "Version planning and changelogs."),
        };

        var teamsToAdd = teamSpecs
            .Where(spec => existing.All(t => !t.Name.Equals(spec.Name, StringComparison.OrdinalIgnoreCase)))
            .Take(MinTeamCount - existing.Count)
            .Select((spec, index) => new Team
        {
            Id = Guid.NewGuid(),
            Name = spec.Name,
            Description = spec.Description,
            CreatedByUserId = admin.Id,
            CreatedAt = seedTime.AddDays(existing.Count + index)
        }).ToList();

        if (teamsToAdd.Count == 0)
        {
            return existing;
        }

        context.Teams.AddRange(teamsToAdd);
        await context.SaveChangesAsync(cancellationToken);
        existing.AddRange(teamsToAdd);
        return existing;
    }

    private static async Task EnsureTeamMembersAsync(
        ApplicationDbContext context,
        IReadOnlyList<Team> teams,
        IReadOnlyList<User> users,
        DateTime seedTime,
        CancellationToken cancellationToken)
    {
        var existingPairs = await context.TeamMembers
            .Select(m => new { m.TeamId, m.UserId })
            .ToListAsync(cancellationToken);
        var existingSet = existingPairs
            .Select(p => (p.TeamId, p.UserId))
            .ToHashSet();

        var teamsNeedingMembers = teams
            .Where(t => existingPairs.All(p => p.TeamId != t.Id))
            .ToList();

        if (teamsNeedingMembers.Count == 0 &&
            await context.TeamMembers.CountAsync(cancellationToken) >= MinTaskCount)
        {
            return;
        }

        var targetTeams = teamsNeedingMembers.Count > 0 ? teamsNeedingMembers : teams.ToList();
        var managers = users.Where(u => u.Role == UserRole.Manager).ToList();
        var regularUsers = users.Where(u => u.Role == UserRole.User).ToList();
        var members = new List<TeamMember>();
        var joinedAt = seedTime.AddDays(teams.Count);

        for (var teamIndex = 0; teamIndex < targetTeams.Count; teamIndex++)
        {
            var team = targetTeams[teamIndex];
            var manager = managers[teamIndex % managers.Count];
            if (existingSet.Add((team.Id, manager.Id)))
            {
                members.Add(new TeamMember
                {
                    TeamId = team.Id,
                    UserId = manager.Id,
                    JoinedAt = joinedAt
                });
                joinedAt = joinedAt.AddHours(1);
            }

            var usersForTeam = regularUsers
                .Skip((teamIndex * 3) % Math.Max(regularUsers.Count - 3, 1))
                .Take(3)
                .ToList();

            foreach (var user in usersForTeam)
            {
                if (!existingSet.Add((team.Id, user.Id)))
                {
                    continue;
                }

                members.Add(new TeamMember
                {
                    TeamId = team.Id,
                    UserId = user.Id,
                    JoinedAt = joinedAt
                });
                joinedAt = joinedAt.AddMinutes(30);
            }
        }

        var memberTotal = existingPairs.Count + members.Count;
        var fillIndex = 0;
        while (memberTotal < MinTaskCount + 5 && fillIndex < teams.Count * regularUsers.Count)
        {
            var team = teams[fillIndex % teams.Count];
            var user = regularUsers[fillIndex % regularUsers.Count];
            fillIndex++;
            if (!existingSet.Add((team.Id, user.Id)))
            {
                continue;
            }

            members.Add(new TeamMember
            {
                TeamId = team.Id,
                UserId = user.Id,
                JoinedAt = joinedAt
            });
            joinedAt = joinedAt.AddMinutes(15);
            memberTotal++;
        }

        if (members.Count == 0)
        {
            return;
        }

        context.TeamMembers.AddRange(members);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static List<TaskItem> CreateTasks(
        IReadOnlyList<Team> teams,
        IReadOnlyList<User> users,
        User admin,
        DateTime seedTime)
    {
        var managers = users.Where(u => u.Role == UserRole.Manager).ToList();
        var assignees = users.Where(u => u.Role != UserRole.Admin).ToList();
        var statuses = new[] { DomainTaskStatus.ToDo, DomainTaskStatus.InProgress, DomainTaskStatus.Done };
        var priorities = new[] { TaskPriority.Low, TaskPriority.Medium, TaskPriority.High };

        var titleTemplates = new[]
        {
            "Migrate login flow to JWT refresh tokens",
            "Design task board kanban view",
            "Document team permission matrix",
            "Set up staging environment alerts",
            "Automate regression suite for tasks API",
            "Build weekly velocity dashboard",
            "Refactor user listing filters",
            "Add comment notifications",
            "Harden password reset endpoints",
            "Create onboarding checklist for managers",
            "Optimize SQL indexes on TaskItems",
            "Implement due-date reminder job",
            "Review Swagger examples for tasks",
            "Fix pagination on large team lists",
            "Add audit log for status changes",
            "Prototype mobile-friendly task detail",
            "Align enum values with frontend",
            "Load-test concurrent task updates",
            "Seed script for demo environments",
            "Update README deployment section",
            "Validate Azure SQL firewall runbook",
            "Cross-team dependency mapping",
            "Escalation policy for blocked tasks",
            "Quarterly access review for admins",
            "Backup verification for ManagementSystemDb",
        };

        var tasks = new List<TaskItem>();
        for (var i = 0; i < titleTemplates.Length; i++)
        {
            var team = teams[i % teams.Count];
            var assignee = assignees[i % assignees.Count];
            var creator = managers[i % managers.Count];
            var createdAt = seedTime.AddDays(i / 3).AddHours(i % 8);

            tasks.Add(new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = titleTemplates[i],
                Description = $"Demo task #{i + 1} for {team.Name}.",
                Status = statuses[i % statuses.Length],
                Priority = priorities[i % priorities.Length],
                DueDate = createdAt.AddDays(7 + (i % 14)),
                AssigneeId = assignee.Id,
                CreatedById = creator.Id,
                TeamId = team.Id,
                CreatedAt = createdAt,
                UpdatedAt = createdAt.AddHours(2 + (i % 5))
            });
        }

        return tasks;
    }

    private static List<TaskComment> CreateComments(
        IReadOnlyList<TaskItem> tasks,
        IReadOnlyList<User> users,
        DateTime seedTime)
    {
        var commentAuthors = users.Where(u => u.Role != UserRole.Admin).ToList();
        var bodies = new[]
        {
            "Started initial investigation.",
            "Blocked until API contract is finalized.",
            "Pushed a draft PR for review.",
            "QA found an edge case on assignee change.",
            "Deploying to staging tonight.",
            "Verified fix in local environment.",
            "Need design sign-off before merge.",
            "Updated acceptance criteria in the ticket.",
            "Synced with platform team — on track.",
            "Please re-test after latest migration.",
        };

        var comments = new List<TaskComment>();
        var index = 0;
        foreach (var task in tasks)
        {
            var commentCount = 1 + (index % 2);
            for (var c = 0; c < commentCount; c++)
            {
                var author = commentAuthors[(index + c) % commentAuthors.Count];
                comments.Add(new TaskComment
                {
                    Id = Guid.NewGuid(),
                    TaskId = task.Id,
                    AuthorId = author.Id,
                    Body = bodies[(index + c) % bodies.Length],
                    CreatedAt = task.CreatedAt.AddHours(4 + c)
                });
            }

            index++;
        }

        return comments;
    }

    private static List<Notification> CreateNotifications(
        IReadOnlyList<TaskItem> tasks,
        IReadOnlyList<User> users,
        DateTime seedTime)
    {
        var notifications = new List<Notification>();
        var recipients = users.Where(u => u.Role != UserRole.Admin).ToList();

        for (var i = 0; i < 25; i++)
        {
            var task = tasks[i % tasks.Count];
            var user = recipients[i % recipients.Count];
            var isAssignment = i % 2 == 0;

            notifications.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Type = isAssignment ? NotificationType.TaskAssigned : NotificationType.TaskStatusUpdated,
                Message = isAssignment
                    ? $"You were assigned: {task.Title}"
                    : $"Status updated on: {task.Title}",
                RelatedTaskId = task.Id,
                IsRead = i % 5 == 0,
                CreatedAt = seedTime.AddDays(i / 2).AddHours(i % 12)
            });
        }

        return notifications;
    }

    private static User CreateUser(
        string email,
        string fullName,
        UserRole role,
        string password,
        PasswordHasher<User> hasher,
        DateTime createdAt)
    {
        var entity = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FullName = fullName,
            Role = role,
            CreatedAt = createdAt,
            PasswordHash = string.Empty
        };

        entity.PasswordHash = hasher.HashPassword(entity, password);
        return entity;
    }
}
