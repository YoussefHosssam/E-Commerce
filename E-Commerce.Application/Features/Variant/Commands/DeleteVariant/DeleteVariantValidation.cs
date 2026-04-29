using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Variant.Commands.DeleteVariant;

public sealed class DeleteVariantValidation : AbstractValidator<DeleteVariantCommand>
{
    public DeleteVariantValidation()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithError(ErrorCodes.Variant.ProductRequired);
        RuleFor(x => x.VariantId).NotEmpty().WithError(ErrorCodes.Variant.VariantIdRequired);
    }
}
