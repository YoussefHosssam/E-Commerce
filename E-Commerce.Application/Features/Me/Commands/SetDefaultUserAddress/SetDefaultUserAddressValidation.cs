using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Me.Commands.SetDefaultUserAddress;

public sealed class SetDefaultUserAddressValidation : AbstractValidator<SetDefaultUserAddressCommand>
{
    public SetDefaultUserAddressValidation()
    {
        RuleFor(x => x.AddressId).NotEmpty().WithError(UserAddressErrors.NotFound);
    }
}
