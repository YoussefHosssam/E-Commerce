using E_Commerce.Application.Contracts.Persistence;
using E_Commerce.Domain.Entities;
using E_Commerce.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Persistence.Repositories;

internal sealed class UserProfileRepository : IUserProfileRepository
{
    private readonly DbSet<UserProfile> _profiles;

    public UserProfileRepository(EcommerceContext context)
    {
        _profiles = context.Set<UserProfile>();
    }

    public async Task<UserProfile?> GetByUserIdAsync(Guid userId, bool tracking, CancellationToken ct)
    {
        var query = tracking ? _profiles : _profiles.AsNoTracking();
        return await query.FirstOrDefaultAsync(x => x.UserId == userId, ct);
    }

    public async Task CreateAsync(UserProfile profile, CancellationToken ct)
        => await _profiles.AddAsync(profile, ct);
}
