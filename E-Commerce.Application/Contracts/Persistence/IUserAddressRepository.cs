using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Contracts.Persistence;

public interface IUserAddressRepository
{
    Task<IReadOnlyCollection<UserAddress>> GetByUserIdAsync(Guid userId, bool tracking, CancellationToken ct);
    Task<UserAddress?> GetByIdAsync(Guid addressId, bool tracking, CancellationToken ct);
    Task<UserAddress?> GetByIdForUserAsync(Guid userId, Guid addressId, bool tracking, CancellationToken ct);
    Task<bool> AnyForUserAsync(Guid userId, CancellationToken ct);
    Task CreateAsync(UserAddress address, CancellationToken ct);
    void Delete(UserAddress address);
}
