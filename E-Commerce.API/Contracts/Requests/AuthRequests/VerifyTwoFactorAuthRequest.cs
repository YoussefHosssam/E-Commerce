
namespace E_Commerce.API.Contracts.Requests.AuthRequests
{
    public sealed record VerifyTwoFactorAuthRequest(string OtpCode);
}