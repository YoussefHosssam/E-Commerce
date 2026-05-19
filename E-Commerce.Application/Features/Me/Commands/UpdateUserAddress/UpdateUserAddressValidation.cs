using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Me.Commands.UpdateUserAddress;

public sealed class UpdateUserAddressValidation : AbstractValidator<UpdateUserAddressCommand>
{
    public UpdateUserAddressValidation()
    {
        RuleFor(x => x.AddressId).NotEmpty().WithError(UserAddressErrors.NotFound);
        RuleFor(x => x.Label).IsInEnum().WithError(UserAddressErrors.LabelInvalid);
        RuleFor(x => x.Country).NotEmpty().WithError(UserAddressErrors.CountryRequired).MaximumLength(100).WithError(UserAddressErrors.TextTooLong);
        RuleFor(x => x.Governorate).NotEmpty().WithError(UserAddressErrors.GovernorateRequired).MaximumLength(100).WithError(UserAddressErrors.TextTooLong);
        RuleFor(x => x.City).NotEmpty().WithError(UserAddressErrors.CityRequired).MaximumLength(100).WithError(UserAddressErrors.TextTooLong);
        RuleFor(x => x.Area).NotEmpty().WithError(UserAddressErrors.AreaRequired).MaximumLength(150).WithError(UserAddressErrors.TextTooLong);
        RuleFor(x => x.Street).NotEmpty().WithError(UserAddressErrors.StreetRequired).MaximumLength(200).WithError(UserAddressErrors.TextTooLong);
        RuleFor(x => x.BuildingNumber).NotEmpty().WithError(UserAddressErrors.BuildingNumberRequired).MaximumLength(50).WithError(UserAddressErrors.TextTooLong);
        RuleFor(x => x.Floor).MaximumLength(50).WithError(UserAddressErrors.TextTooLong);
        RuleFor(x => x.Apartment).MaximumLength(50).WithError(UserAddressErrors.TextTooLong);
        RuleFor(x => x.PostalCode).MaximumLength(30).WithError(UserAddressErrors.TextTooLong);
        RuleFor(x => x.Landmark).MaximumLength(300).WithError(UserAddressErrors.TextTooLong);
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).WithError(UserAddressErrors.LatitudeInvalid).When(x => x.Latitude.HasValue);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).WithError(UserAddressErrors.LongitudeInvalid).When(x => x.Longitude.HasValue);
    }
}
