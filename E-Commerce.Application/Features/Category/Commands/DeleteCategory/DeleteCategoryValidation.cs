using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Category.Commands;

public sealed class DeleteCategoryValidation : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryValidation()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithError(CategoryErrors.IdRequired);
    }
}
