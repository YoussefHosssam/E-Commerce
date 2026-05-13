using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Application.Contracts.Infrastructure.Emails.Models;

public record ResetPasswordEmailModel(string FullName, EmailAddress Email, string Token)
{
    public string ResetPasswordUrl => $"https://localhost/api/v1/auth/password/reset?token={Token}";
}
