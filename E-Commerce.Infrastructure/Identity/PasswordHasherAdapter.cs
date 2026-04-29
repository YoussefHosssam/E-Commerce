using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Domain.ValueObjects;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Identity
{
    public class PasswordHasherAdapter : IPasswordHasherAdapter
    {
        private readonly PasswordHasher _hasher = new();

        public PasswordHash Hash(string password)
        {
            string hashed = _hasher.HashPassword(password);
            return PasswordHash.Create(hashed);
        }

        public bool Verify(PasswordHash hash, string password) =>
            _hasher.VerifyHashedPassword(hash.Value, password) != PasswordVerificationResult.Failed;
    }
}
