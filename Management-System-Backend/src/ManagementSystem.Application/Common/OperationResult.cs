namespace ManagementSystem.Application.Common;

public sealed class OperationResult<T>
{
    public T? Value { get; init; }

    public OperationFailureKind? Failure { get; init; }

    public bool IsSuccess => Failure is null;

    public static OperationResult<T> Success(T value) => new() { Value = value };

    public static OperationResult<T> Fail(OperationFailureKind failure) => new() { Failure = failure };
}
