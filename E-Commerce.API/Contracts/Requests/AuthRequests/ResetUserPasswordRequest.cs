namespace E_Commerce.API.Contracts.Requests.AuthRequests;

public sealed record ResetUserPasswordRequest(string Token, string NewPassword);
