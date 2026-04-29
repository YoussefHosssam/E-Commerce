using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Variant.Commands.UpdateVariant;

public sealed class UpdateVariantValidation : AbstractValidator<UpdateVariantCommand>
{
    public UpdateVariantValidation()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithError(ErrorCodes.Variant.ProductRequired);
        RuleFor(x => x.VariantId).NotEmpty().WithError(ErrorCodes.Variant.VariantIdRequired);
        RuleFor(x => x.Sku).NotEmpty().WithError(ErrorCodes.Variant.SkuRequired);
        RuleFor(x => x.PriceOverrideAmount).GreaterThanOrEqualTo(0).When(x => x.PriceOverrideAmount.HasValue).WithError(ErrorCodes.Variant.PriceInvalid);
        RuleFor(x => x.PriceOverrideCurrency).Length(3).When(x => !string.IsNullOrWhiteSpace(x.PriceOverrideCurrency)).WithError(ErrorCodes.Variant.CurrencyInvalid);
        RuleFor(x => x).Must(x => x.PriceOverrideAmount.HasValue == !string.IsNullOrWhiteSpace(x.PriceOverrideCurrency)).WithError(ErrorCodes.Variant.PriceRequired);
    }
}

