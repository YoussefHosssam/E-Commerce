using E_Commerce.Application.Extensions;
using E_Commerce.Application.Features.Variant.Common;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

public sealed class VariantCreateDtoValidator : AbstractValidator<VariantCreateDto>
{
    public VariantCreateDtoValidator()
    {
        RuleFor(x => x.Sku)
            .NotEmpty()
            .WithError(VariantErrors.SkuRequired);

        When(x => x.VariantPriceOverrideAmount.HasValue, () =>
        {
            RuleFor(x => x.VariantPriceOverrideAmount)
                .GreaterThan(0)
                .WithError(VariantErrors.PriceInvalid);
        });

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0)
            .WithError(InventoryErrors.InitialQuantityInvalid);

        RuleFor(x => x.Color)
            .NotNull()
            .WithError(VariantErrors.ColorRequired);

        When(x => x.Color is not null, () =>
        {
            RuleFor(x => x.Color!.Name)
                .NotEmpty()
                .WithError(VariantErrors.ColorNameRequired);

            RuleFor(x => x.Color!.HexCode)
                .NotEmpty()
                .WithError(VariantErrors.ColorHexCodeRequired);
        });
    }
}
