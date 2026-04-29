using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Product.Queries.GetProducts;

public sealed class GetProductsValidation : AbstractValidator<GetProductsQuery>
{
    public GetProductsValidation()
    {
        RuleFor(x => x.page.PageNumber)
            .GreaterThan(0)
            .WithError(ErrorCodes.Common.PageInvalid);

        RuleFor(x => x.page.PageSize)
            .GreaterThan(0)
            .WithError(ErrorCodes.Common.PageSizeInvalid);
    }
}
