namespace ManagementSystem.Application.Common;

public sealed class AuthResult<T>
{
    public T? Value { get; init; }

    public AuthFailureKind? Failure { get; init; }

    public bool IsSuccess => Failure is null;

    public static AuthResult<T> Success(T value) => new() { Value = value };

    public static AuthResult<T> Fail(AuthFailureKind failure) => new() { Failure = failure };
}
