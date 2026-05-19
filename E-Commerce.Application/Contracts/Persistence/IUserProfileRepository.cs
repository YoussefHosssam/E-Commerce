using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Contracts.Persistence;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByUserIdAsync(Guid userId, bool tracking, CancellationToken ct);
    Task CreateAsync(UserProfile profile, CancellationToken ct);
}
