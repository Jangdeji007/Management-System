namespace ManagementSystem.Api;

public sealed class UnauthorizedUserException(string message) : InvalidOperationException(message);
