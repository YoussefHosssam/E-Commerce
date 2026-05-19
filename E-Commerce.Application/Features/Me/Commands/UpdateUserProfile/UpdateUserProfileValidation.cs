using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Me.Commands.UpdateUserProfile;

public sealed class UpdateUserProfileValidation : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileValidation()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithError(UserProfileErrors.FirstNameRequired)
            .MaximumLength(100).WithError(UserProfileErrors.NameTooLong);

        RuleFor(x => x.LastName)
            .NotEmpty().WithError(UserProfileErrors.LastNameRequired)
            .MaximumLength(100).WithError(UserProfileErrors.NameTooLong);

        RuleFor(x => x.DisplayName)
            .MaximumLength(150).WithError(UserProfileErrors.DisplayNameTooLong)
            .When(x => x.DisplayName is not null);

        RuleFor(x => x.Gender)
            .IsInEnum().WithError(UserProfileErrors.GenderInvalid)
            .When(x => x.Gender.HasValue);

        RuleFor(x => x.DateOfBirth)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow)).WithError(UserProfileErrors.DateOfBirthInvalid)
            .When(x => x.DateOfBirth.HasValue);

        RuleFor(x => x.AvatarUrl)
            .Must(BeHttpUrl).WithError(UserProfileErrors.AvatarUrlInvalid)
            .When(x => !string.IsNullOrWhiteSpace(x.AvatarUrl));
    }

    private static bool BeHttpUrl(string? value)
        => Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Scheme is "http" or "https";
}
