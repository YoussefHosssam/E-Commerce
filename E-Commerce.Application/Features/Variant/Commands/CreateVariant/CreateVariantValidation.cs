using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Variant.Commands.CreateVariant;

public sealed class CreateVariantValidation : AbstractValidator<CreateVariantCommand>
{
    public CreateVariantValidation()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithError(VariantErrors.ProductRequired);
        RuleFor(x => x.Variants).NotEmpty().WithError(VariantErrors.ProductMustHaveAtLeastOneVariant);
        RuleForEach(x => x.Variants).ChildRules(variant =>
        {
            variant.RuleFor(x => x.Sku).NotEmpty().WithError(VariantErrors.SkuRequired);
            variant.RuleFor(x => x.VariantPriceOverrideAmount).GreaterThan(0).When(x => x.VariantPriceOverrideAmount.HasValue).WithError(VariantErrors.PriceInvalid);
            variant.RuleFor(x => x.Stock).GreaterThanOrEqualTo(0).WithError(VariantErrors.PriceInvalid);
            variant.RuleFor(x => x.Color).NotNull().WithError(VariantErrors.ColorRequired);
            variant.When(x => x.Color is not null, () =>
            {
                variant.RuleFor(x => x.Color!.Name).NotEmpty().WithError(VariantErrors.ColorNameRequired);
                variant.RuleFor(x => x.Color!.HexCode).NotEmpty().WithError(VariantErrors.ColorHexCodeRequired);
                variant.RuleFor(x => x.Color!.HexCode).Matches("^#[0-9A-Fa-f]{6}$").WithError(VariantErrors.ColorHexCodeInvalid);
            });
        });
    }
}
