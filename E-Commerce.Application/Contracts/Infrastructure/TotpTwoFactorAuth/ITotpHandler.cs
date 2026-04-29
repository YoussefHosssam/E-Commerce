using E_Commerce.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Infrastructure.TotpTwoFactorAuth
{
    public interface ITotpHandler
    {
        Task<string> GenerateQrCodeAsync(string manualKey, EmailAddress email, CancellationToken ctn);
        (string secretKey, string encryptedSecretKey) GenerateSecretKey(int bytes = 20);
        bool VerifyCode(string encryptedSecretKey, string code);
    }
}
