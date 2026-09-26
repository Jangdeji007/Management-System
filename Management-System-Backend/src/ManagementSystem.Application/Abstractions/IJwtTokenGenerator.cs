using ManagementSystem.Domain.Entities;

namespace ManagementSystem.Application.Abstractions;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user);
}
