using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Me.Commands.DeleteUserAddress;

public sealed class DeleteUserAddressValidation : AbstractValidator<DeleteUserAddressCommand>
{
    public DeleteUserAddressValidation()
    {
        RuleFor(x => x.AddressId).NotEmpty().WithError(UserAddressErrors.NotFound);
    }
}
