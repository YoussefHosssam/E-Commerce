using E_Commerce.Application.Contracts.Persistence;
using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.ValueObjects;
using FluentValidation;

namespace E_Commerce.Application.Features.Auth.Commands.RegisterUser
{
    public sealed class RegisterUserValidation : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserValidation(IUserRepository userRepository)
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.FirstName)
                .NotEmpty().WithError(UserErrors.FirstNameRequired)
                .Must(name => !string.IsNullOrWhiteSpace(name)).WithError(UserErrors.FirstNameRequired)
                .Must(name => name == null || name.Trim().Length >= 2).WithError(UserErrors.FirstNameTooShort)
                .Must(name => name == null || name.Trim().Length <= 100).WithError(UserErrors.FirstNameTooLong);

            RuleFor(x => x.LastName)
                .NotEmpty().WithError(UserErrors.LastNameRequired)
                .Must(name => !string.IsNullOrWhiteSpace(name)).WithError(UserErrors.LastNameRequired)
                .Must(name => name == null || name.Trim().Length >= 2).WithError(UserErrors.LastNameTooShort)
                .Must(name => name == null || name.Trim().Length <= 100).WithError(UserErrors.LastNameTooLong);

            RuleFor(x => x.Password)
                .NotEmpty().WithError(AuthErrors.PasswordRequired)
                .MinimumLength(8).WithError(AuthErrors.PasswordTooShort)
                .MaximumLength(128).WithError(AuthErrors.PasswordTooLong)
                .Matches("[A-Z]").WithError(AuthErrors.PasswordUppercaseMissing)
                .Matches("[a-z]").WithError(AuthErrors.PasswordLowercaseMissing)
                .Matches("[0-9]").WithError(AuthErrors.PasswordDigitMissing)
                .Matches(@"[\W_]").WithError(AuthErrors.PasswordSpecialCharacterMissing)
                .Must(password => password == null || !password.Any(char.IsWhiteSpace)).WithError(AuthErrors.PasswordWhitespaceInvalid);

            RuleFor(x => x.Email)
                .NotEmpty().WithError(UserErrors.EmailRequired)
                .EmailAddress().WithError(UserErrors.EmailInvalid)
                .MaximumLength(256).WithError(UserErrors.EmailTooLong)
                .MustAsync(async (email, cancellationToken) =>
                {
                    if (string.IsNullOrWhiteSpace(email)) return true;
                    return !await userRepository.IsEmailExist(EmailAddress.Create(email), cancellationToken);
                }).WithError(UserErrors.EmailAlreadyExists);

            When(x => !string.IsNullOrEmpty(x.PhoneNumber), () =>
            {
                RuleFor(x => x.PhoneNumber)
                   .NotEmpty().WithError(UserErrors.PhoneRequired)
                   .Must(phone => !string.IsNullOrWhiteSpace(phone)).WithError(UserErrors.PhoneRequired)
                   .Must(phone => phone == null || phone.Trim().Length <= 30).WithError(UserErrors.PhoneTooLong)
                   .Matches(@"^\+?[0-9\s\-\(\)]+$").WithError(UserErrors.PhoneInvalid);
            });

        }
    }
}
