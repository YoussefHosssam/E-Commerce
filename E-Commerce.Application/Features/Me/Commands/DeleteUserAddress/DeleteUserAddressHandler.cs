using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.API.Identity;
using E_Commerce.Domain.Common.Errors;
using MediatR;

namespace E_Commerce.Application.Features.Me.Commands.DeleteUserAddress;

public sealed class DeleteUserAddressHandler : IRequestHandler<DeleteUserAddressCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly IUserAccessor _userAccessor;

    public DeleteUserAddressHandler(IUnitOfWork uow, IUserAccessor userAccessor)
    {
        _uow = uow;
        _userAccessor = userAccessor;
    }

    public async Task<Result> Handle(DeleteUserAddressCommand request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.GetRequiredUserId();
        var user = await _uow.Users.GetByIdWithLoadingDataAsync(userId, cancellationToken);
        if (user is null)
            return Result.Fail(UserErrors.NotFound);

        var addresses = await _uow.UserAddresses.GetByUserIdAsync(userId, true, cancellationToken);
        var address = addresses.FirstOrDefault(x => x.Id == request.AddressId);
        if (address is null)
            return Result.Fail(UserAddressErrors.NotFound);

        // Deleting the default address intentionally leaves the user with no default address.
        // This avoids filtered unique-index ordering conflicts and lets the user explicitly choose the next default.
        _uow.UserAddresses.Delete(address);

        await _uow.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
