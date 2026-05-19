using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Application.Features.Me.Common;
using E_Commerce.Domain.Common.Errors;
using MediatR;

namespace E_Commerce.Application.Features.Me.Queries.GetCurrentUser;

public sealed class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, Result<CurrentUserDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IUserAccessor _userAccessor;

    public GetCurrentUserHandler(IUnitOfWork uow, IUserAccessor userAccessor)
    {
        _uow = uow;
        _userAccessor = userAccessor;
    }

    public async Task<Result<CurrentUserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;
        if (!userId.HasValue)
            return Result<CurrentUserDto>.Fail(AuthErrors.InvalidToken);

        var user = await _uow.Users.GetByIdWithLoadingDataAsync(userId.Value, cancellationToken);
        if (user is null)
            return Result<CurrentUserDto>.Fail(UserErrors.NotFound);

        var profile = await _uow.UserProfiles.GetByUserIdAsync(user.Id, false, cancellationToken);
        var profileDto = profile is null
            ? new UserProfileDto(user.Id, user.FirstName, user.LastName, $"{user.FirstName} {user.LastName}".Trim(), null, null, null)
            : new UserProfileDto(
                profile.UserId,
                profile.FirstName,
                profile.LastName,
                profile.DisplayName,
                profile.Gender,
                profile.DateOfBirth,
                profile.AvatarUrl);

        var dto = new CurrentUserDto(
            user.Id,
            user.Email.Value,
            user.FirstName,
            user.LastName,
            profileDto.DisplayName,
            user.Phone,
            user.Role.ToString(),
            profileDto);

        return Result<CurrentUserDto>.Success(dto);
    }
}
