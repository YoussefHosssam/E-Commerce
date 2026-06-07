using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.API.Identity;
using E_Commerce.Application.Features.Me.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using MediatR;

namespace E_Commerce.Application.Features.Me.Commands.UpdateUserProfile;

public sealed class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileCommand, Result<UserProfileDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IUserAccessor _userAccessor;

    public UpdateUserProfileHandler(IUnitOfWork uow, IUserAccessor userAccessor)
    {
        _uow = uow;
        _userAccessor = userAccessor;
    }

    public async Task<Result<UserProfileDto>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.GetRequiredUserId();
        var user = await _uow.Users.GetByIdWithLoadingDataAsync(userId, cancellationToken);
        if (user is null)
            return Result<UserProfileDto>.Fail(UserErrors.NotFound);

        var now = DateTimeOffset.UtcNow;
        var profile = await _uow.UserProfiles.GetByUserIdAsync(user.Id, true, cancellationToken);

        if (profile is null)
        {
            profile = UserProfile.Create(user.Id, request.FirstName, request.LastName, request.DisplayName);
            profile.Update(request.FirstName, request.LastName, request.DisplayName, request.Gender, request.DateOfBirth, request.AvatarUrl, now);
            await _uow.UserProfiles.CreateAsync(profile, cancellationToken);
        }
        else
        {
            profile.Update(request.FirstName, request.LastName, request.DisplayName, request.Gender, request.DateOfBirth, request.AvatarUrl, now);
        }

        user.ChangeName(profile.FirstName, profile.LastName);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<UserProfileDto>.Success(new UserProfileDto(
            profile.UserId,
            profile.FirstName,
            profile.LastName,
            profile.DisplayName,
            profile.Gender,
            profile.DateOfBirth,
            profile.AvatarUrl));
    }
}
