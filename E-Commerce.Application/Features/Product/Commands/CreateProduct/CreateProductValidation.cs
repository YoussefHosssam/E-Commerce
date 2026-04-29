using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Product.Commands.CreateProduct;

public sealed class CreateProductValidation : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidation()
    {
        RuleFor(x => x.CategoryId).NotEmpty().WithError(ErrorCodes.Product.CategoryRequired);
        RuleFor(x => x.Slug).NotEmpty().WithError(ErrorCodes.Product.SlugRequired);
        RuleFor(x => x.BasePriceAmount).GreaterThanOrEqualTo(0).WithError(ErrorCodes.Product.BasePriceInvalid);
        RuleFor(x => x.BasePriceCurrency).NotEmpty().Length(3).WithError(ErrorCodes.Product.CurrencyInvalid);
        RuleFor(x => x.Status).IsInEnum().WithError(ErrorCodes.Product.StatusInvalid);
    }
}
