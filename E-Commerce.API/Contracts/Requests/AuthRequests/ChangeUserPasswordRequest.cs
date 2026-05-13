namespace E_Commerce.API.Contracts.Requests.AuthRequests;

public sealed record ChangeUserPasswordRequest(string CurrentPassword, string NewPassword);
