using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Persistence
{
    public interface IUserRepository : IGenericRepository<User>
    {
        public Task<bool> IsEmailExist(EmailAddress email , CancellationToken ct);
        public Task<User?> GetUserByEmailAsync(EmailAddress email, CancellationToken ct);
        public Task<User?> GetUserByEmailWithTrackingAsync(EmailAddress email, CancellationToken ct);
        public Task<User?> GetByIdWithLoadingDataAsync(Guid UserId, CancellationToken ct);

    }
}
