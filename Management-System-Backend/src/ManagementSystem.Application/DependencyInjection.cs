using ManagementSystem.Application.Services.Auth;
using ManagementSystem.Application.Services.Dashboard;
using ManagementSystem.Application.Services.Notifications;
using ManagementSystem.Application.Services.Tasks;
using ManagementSystem.Application.Services.Teams;
using ManagementSystem.Application.Services.Users;
using Microsoft.Extensions.DependencyInjection;

namespace ManagementSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITeamService, TeamService>();
        services.AddScoped<TaskAccessEvaluator>();
        services.AddScoped<TaskQueryScopeBuilder>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<ITaskCommentService, TaskCommentService>();
        services.AddScoped<ITaskNotificationService, TaskNotificationService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IUserQueryService, UserQueryService>();
        return services;
    }
}
