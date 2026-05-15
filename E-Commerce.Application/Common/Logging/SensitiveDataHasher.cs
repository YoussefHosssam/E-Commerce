using System.Security.Cryptography;
using System.Text;

namespace E_Commerce.Application.Common.Logging;

public static class SensitiveDataHasher
{
    public static string HashEmail(string email)
    {
        var normalized = email.Trim().ToUpperInvariant();
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));

        return Convert.ToHexString(bytes);
    }
}
