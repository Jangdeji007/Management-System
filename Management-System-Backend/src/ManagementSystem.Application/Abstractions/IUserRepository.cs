using ManagementSystem.Domain.Entities;

namespace ManagementSystem.Application.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> ListAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> ListByTeamIdsAsync(
        IReadOnlyCollection<Guid> teamIds,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Guid>> GetTeamIdsForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
