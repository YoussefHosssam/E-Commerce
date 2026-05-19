using E_Commerce.Domain.Enums;

namespace E_Commerce.API.Contracts.Requests.MeRequests;

public sealed record UpdateUserProfileRequest(
    string FirstName,
    string LastName,
    string? DisplayName,
    Gender? Gender,
    DateOnly? DateOfBirth,
    string? AvatarUrl);
