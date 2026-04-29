namespace E_Commerce.API.Contracts.Requests.AuthRequests
{
    public sealed record CompleteLoginWithTwoFactorRequest(string ChallengeId, string OtpCode);
}

