using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Product.Commands.CreateProduct;

public sealed class CreateProductValidation : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidation()
    {
        RuleFor(x => x.Name).NotEmpty().WithError(ProductErrors.NameInvalid);
        RuleFor(x => x.CategoryId).NotEmpty().WithError(ProductErrors.CategoryRequired);
        RuleFor(x => x.Slug).NotEmpty().WithError(ProductErrors.SlugRequired);
        RuleFor(x => x.BasePriceAmount).GreaterThanOrEqualTo(0).WithError(ProductErrors.BasePriceInvalid);
        RuleFor(x => x.BasePriceCurrency).NotEmpty().Length(3).WithError(ProductErrors.CurrencyInvalid);
        RuleFor(x => x.Status).IsInEnum().WithError(ProductErrors.StatusInvalid);
    }
}
