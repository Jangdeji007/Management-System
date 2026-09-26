namespace ManagementSystem.Application.Authorization;

public static class AuthorizationPolicies
{
    public const string AdminOnly = "AdminOnly";

    public const string ManagerOrAdmin = "ManagerOrAdmin";

    public const string Authenticated = "Authenticated";
}
