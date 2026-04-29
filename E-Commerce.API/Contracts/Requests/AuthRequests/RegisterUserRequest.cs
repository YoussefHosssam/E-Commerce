namespace E_Commerce.API.Controllers.V1;

public sealed partial class AuthController
{
    public sealed record RegisterUserRequest(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string? PhoneNumber);
}
