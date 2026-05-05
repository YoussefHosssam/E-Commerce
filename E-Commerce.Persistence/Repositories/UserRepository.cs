using E_Commerce.Application.Contracts.Persistence;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Persistence.Context;
using E_Commerce.Persistence.Repositories.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Persistence.Repositories
{
    internal class UserRepository : GenericRepository<User> , IUserRepository
    {
        private readonly DbSet<User> _users;
        public UserRepository(EcommerceContext ecommerceContext) : base(ecommerceContext)
        {
            _users = ecommerceContext.Set<User>();
        }

        public async Task<User?> GetUserByEmailAsync(EmailAddress email, CancellationToken ct)
        {
            return await _users.Include(u => u.Credential).Include(u => u.TwoFactorAuth).AsNoTracking().FirstOrDefaultAsync(u => u.Email == email && u.VerificationStatus == VerificationStatus.Verified && u.IsActive == true, cancellationToken: ct);
        }

        public async Task<User?> GetUserByEmailWithTrackingAsync(EmailAddress email, CancellationToken ct)
        {
            return await _users.Include(u => u.Credential).Include(u => u.TwoFactorAuth).FirstOrDefaultAsync(u => u.Email == email && u.VerificationStatus == VerificationStatus.Verified && u.IsActive == true, cancellationToken: ct);
        }

        public async Task<User?> GetByIdWithLoadingDataAsync(Guid UserId, CancellationToken ct)
        {
            return await _users.Include(u => u.Credential).Include(u => u.TwoFactorAuth).FirstOrDefaultAsync(u => u.Id == UserId && u.VerificationStatus == VerificationStatus.Verified && u.IsActive == true, cancellationToken: ct);
        }

        public async Task<bool> IsEmailExist(EmailAddress email , CancellationToken ct)
        {
            return await _users.AsNoTracking().AnyAsync(u => u.Email == email, cancellationToken: ct);
        }
    }
}
