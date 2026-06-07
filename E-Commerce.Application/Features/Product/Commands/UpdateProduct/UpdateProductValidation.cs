using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Product.Commands.UpdateProduct;

public sealed class UpdateProductValidation : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidation()
    {
        RuleFor(x => x.Id).NotEmpty().WithError(ProductErrors.IdRequired);
        RuleFor(x => x.Name).NotEmpty().WithError(ProductErrors.NameInvalid);
        RuleFor(x => x.CategoryId).NotEmpty().WithError(ProductErrors.CategoryRequired);
        RuleFor(x => x.Slug).NotEmpty().WithError(ProductErrors.SlugRequired);
        RuleFor(x => x.BasePriceAmount).GreaterThanOrEqualTo(0).WithError(ProductErrors.BasePriceInvalid);
        RuleFor(x => x.BasePriceCurrency).NotEmpty().Length(3).WithError(ProductErrors.CurrencyInvalid);
        RuleFor(x => x.CompareAtPriceAmount).NotNull().When(x => x.HasDiscount).WithError(ProductErrors.DiscountPriceRequired);
        RuleFor(x => x.CompareAtPriceCurrency).NotEmpty().When(x => x.HasDiscount).WithError(ProductErrors.CurrencyRequired);
        RuleFor(x => x.CompareAtPriceAmount).Null().When(x => !x.HasDiscount).WithError(ProductErrors.DiscountPriceNotAllowedWhenHasDiscountFalse);
        RuleFor(x => x.CompareAtPriceCurrency).Null().When(x => !x.HasDiscount).WithError(ProductErrors.DiscountPriceNotAllowedWhenHasDiscountFalse);
        RuleFor(x => x.Variants).Must(x => x is null || x.Count == 1).When(x => !x.HasVariants).WithError(ProductErrors.SimpleProductMustHaveExactlyOneVariant);
        RuleFor(x => x.Status).IsInEnum().WithError(ProductErrors.StatusInvalid);
    }
}
