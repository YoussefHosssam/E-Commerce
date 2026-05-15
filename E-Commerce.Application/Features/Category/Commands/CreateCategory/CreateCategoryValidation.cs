using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Category.Commands;

public sealed class CreateCategoryValidation : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidation()
    {
        RuleFor(x => x.ParentId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithError(CategoryErrors.ParentInvalid);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .WithError(CategoryErrors.SlugRequired);

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0)
            .WithError(CategoryErrors.SortOrderInvalid);

        RuleFor(x => x.Name)
            .Length(5, 50)
            .WithError(CategoryErrors.InvalidName);
        
    }
}
