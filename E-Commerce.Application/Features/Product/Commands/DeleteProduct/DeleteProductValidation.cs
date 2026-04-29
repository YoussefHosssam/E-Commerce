using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Product.Commands.DeleteProduct;

public sealed class DeleteProductValidation : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductValidation()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithError(ErrorCodes.Product.IdRequired);
    }
}
