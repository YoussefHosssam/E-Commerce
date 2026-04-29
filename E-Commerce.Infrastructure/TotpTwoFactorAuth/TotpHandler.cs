using E_Commerce.Application.Contracts.Infrastructure.TotpTwoFactorAuth;
using E_Commerce.Domain.ValueObjects;
using Microsoft.AspNetCore.DataProtection;
using OtpNet;
using QRCoder;
using System;

namespace E_Commerce.Infrastructure.TotpTwoFactorAuth
{
    public class TotpHandler : ITotpHandler
    {
        private readonly IDataProtector _protector;
        private const string Issuer = "YourECommerceApp"; // اسم الأبلكيشن اللي هيظهر في الموبايل

        public TotpHandler(IDataProtectionProvider protectionProvider)
        {
            // تشفير البيانات باستخدام مفتاح خاص بالتطبيق
            _protector = protectionProvider.CreateProtector("2FA.SecretKey.Protection");
        }

        public (string secretKey, string encryptedSecretKey) GenerateSecretKey(int bytes = 20)
        {
            byte[] secretBytes = KeyGeneration.GenerateRandomKey(bytes);
            string manualKey = Base32Encoding.ToString(secretBytes);
            string encryptedBytes = Base32Encoding.ToString(_protector.Protect(secretBytes));
            return (manualKey, encryptedBytes);
        }

        public Task<string> GenerateQrCodeAsync(string manualKey, EmailAddress email, CancellationToken ctn)
        {
            string uri = $"otpauth://totp/{Issuer}:{email.Value}?secret={manualKey}&issuer={Issuer}";
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImage = qrCode.GetGraphic(20);
            string base64Image = Convert.ToBase64String(qrCodeImage);
            return Task.FromResult($"data:image/png;base64,{base64Image}");
        }
        public bool VerifyCode(string encryptedSecretKey, string code)
        {
            try
            {
                code = code.Replace(" ", "").Trim();

                byte[] protectedBytes = Base32Encoding.ToBytes(encryptedSecretKey);
                byte[] decryptedSecret = _protector.Unprotect(protectedBytes);

                var totp = new Totp(decryptedSecret);
                return totp.VerifyTotp(code, out _, VerificationWindow.RfcSpecifiedNetworkDelay);
            }
            catch
            {
                return false;
            }
        }
    }
}