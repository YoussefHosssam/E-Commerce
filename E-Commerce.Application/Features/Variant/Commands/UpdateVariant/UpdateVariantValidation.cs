using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Variant.Commands.UpdateVariant;

public sealed class UpdateVariantValidation : AbstractValidator<UpdateVariantCommand>
{
    public UpdateVariantValidation()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithError(VariantErrors.ProductRequired);
        RuleFor(x => x.VariantId).NotEmpty().WithError(VariantErrors.VariantIdRequired);
        RuleFor(x => x.Sku).NotEmpty().WithError(VariantErrors.SkuRequired);
        RuleFor(x => x.VariantPriceOverrideAmount).NotNull().When(x => x.HasPriceOverride == true).WithError(VariantErrors.PriceRequired);
        RuleFor(x => x.VariantPriceOverrideAmount).GreaterThan(0).When(x => x.HasPriceOverride == true && x.VariantPriceOverrideAmount.HasValue).WithError(VariantErrors.PriceInvalid);
        RuleFor(x => x.VariantPriceOverrideAmount).Null().When(x => x.HasPriceOverride == false).WithError(VariantErrors.InvalidInput);
        RuleFor(x => x.Color).NotNull().WithError(VariantErrors.ColorRequired);
        When(x => x.Color is not null, () =>
        {
            RuleFor(x => x.Color!.Name).NotEmpty().WithError(VariantErrors.ColorNameRequired);
            RuleFor(x => x.Color!.HexCode).NotEmpty().WithError(VariantErrors.ColorHexCodeRequired);
            RuleFor(x => x.Color!.HexCode).Matches("^#[0-9A-Fa-f]{6}$").WithError(VariantErrors.ColorHexCodeInvalid);
        });
    }
}

