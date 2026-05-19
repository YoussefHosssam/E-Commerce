using E_Commerce.Application.Contracts.Persistence;
using E_Commerce.Domain.Entities;
using E_Commerce.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Persistence.Repositories;

internal sealed class UserAddressRepository : IUserAddressRepository
{
    private readonly DbSet<UserAddress> _addresses;

    public UserAddressRepository(EcommerceContext context)
    {
        _addresses = context.Set<UserAddress>();
    }

    public async Task<IReadOnlyCollection<UserAddress>> GetByUserIdAsync(Guid userId, bool tracking, CancellationToken ct)
    {
        var query = tracking ? _addresses : _addresses.AsNoTracking();

        return await query
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<UserAddress?> GetByIdForUserAsync(Guid userId, Guid addressId, bool tracking, CancellationToken ct)
    {
        var query = tracking ? _addresses : _addresses.AsNoTracking();
        return await query.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == addressId, ct);
    }

    public async Task<UserAddress?> GetByIdAsync(Guid addressId, bool tracking, CancellationToken ct)
    {
        var query = tracking ? _addresses : _addresses.AsNoTracking();
        return await query.FirstOrDefaultAsync(x => x.Id == addressId, ct);
    }

    public async Task<bool> AnyForUserAsync(Guid userId, CancellationToken ct)
        => await _addresses.AsNoTracking().AnyAsync(x => x.UserId == userId, ct);

    public async Task CreateAsync(UserAddress address, CancellationToken ct)
        => await _addresses.AddAsync(address, ct);

    public void Delete(UserAddress address)
        => _addresses.Remove(address);
}
