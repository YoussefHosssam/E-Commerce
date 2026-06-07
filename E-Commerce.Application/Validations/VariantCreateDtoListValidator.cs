using E_Commerce.Application.Features.Variant.Common;
using FluentValidation;

public sealed class VariantCreateDtoListValidator : AbstractValidator<IReadOnlyCollection<VariantCreateDto>>
{
    public VariantCreateDtoListValidator()
    {
        RuleForEach(x => x)
            .SetValidator(new VariantCreateDtoValidator());
    }
}