using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Features.Me.Common;
using E_Commerce.Domain.Enums;
using MediatR;

namespace E_Commerce.Application.Features.Me.Commands.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(
    string FirstName,
    string LastName,
    string? DisplayName,
    Gender? Gender,
    DateOnly? DateOfBirth,
    string? AvatarUrl) : IRequest<Result<UserProfileDto>>;
