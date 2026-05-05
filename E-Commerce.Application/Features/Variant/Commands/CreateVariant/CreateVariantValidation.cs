using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Variant.Commands.CreateVariant;

public sealed class CreateVariantValidation : AbstractValidator<CreateVariantCommand>
{
    public CreateVariantValidation()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithError(VariantErrors.ProductRequired);
        RuleFor(x => x.Sku).NotEmpty().WithError(VariantErrors.SkuRequired);
        RuleFor(x => x.PriceOverrideAmount).GreaterThanOrEqualTo(0).When(x => x.PriceOverrideAmount.HasValue).WithError(VariantErrors.PriceInvalid);
        RuleFor(x => x.PriceOverrideCurrency).Length(3).When(x => !string.IsNullOrWhiteSpace(x.PriceOverrideCurrency)).WithError(VariantErrors.CurrencyInvalid);
        RuleFor(x => x).Must(x => x.PriceOverrideAmount.HasValue == !string.IsNullOrWhiteSpace(x.PriceOverrideCurrency)).WithError(VariantErrors.PriceRequired);
    }
}
