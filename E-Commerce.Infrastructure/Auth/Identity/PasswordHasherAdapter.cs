using E_Commerce.Application.Contracts.API.Identity;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth;
using E_Commerce.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Infrastructure.Identity;

internal sealed class PasswordHasherAdapter : IPasswordHasherAdapter
{
    private readonly PasswordHasher<object> _hasher = new();

    public PasswordHash Hash(string password)
    {
        var hashedPassword = _hasher.HashPassword(
            new object(),
            password);

        return PasswordHash.Create(hashedPassword);
    }

    public bool Verify(
        PasswordHash hash,
        string password)
    {
        var result = _hasher.VerifyHashedPassword(
            new object(),
            hash.Value,
            password);

        return result == PasswordVerificationResult.Success
            || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}