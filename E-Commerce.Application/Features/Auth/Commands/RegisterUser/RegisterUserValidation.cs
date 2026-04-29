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
                .NotEmpty().WithError(ErrorCodes.User.FirstNameRequired)
                .Must(name => !string.IsNullOrWhiteSpace(name)).WithError(ErrorCodes.User.FirstNameRequired)
                .Must(name => name == null || name.Trim().Length >= 2).WithError(ErrorCodes.User.FirstNameTooShort)
                .Must(name => name == null || name.Trim().Length <= 100).WithError(ErrorCodes.User.FirstNameTooLong);

            RuleFor(x => x.LastName)
                .NotEmpty().WithError(ErrorCodes.User.LastNameRequired)
                .Must(name => !string.IsNullOrWhiteSpace(name)).WithError(ErrorCodes.User.LastNameRequired)
                .Must(name => name == null || name.Trim().Length >= 2).WithError(ErrorCodes.User.LastNameTooShort)
                .Must(name => name == null || name.Trim().Length <= 100).WithError(ErrorCodes.User.LastNameTooLong);

            RuleFor(x => x.Password)
                .NotEmpty().WithError(ErrorCodes.Auth.PasswordRequired)
                .MinimumLength(8).WithError(ErrorCodes.Auth.PasswordTooShort)
                .MaximumLength(128).WithError(ErrorCodes.Auth.PasswordTooLong)
                .Matches("[A-Z]").WithError(ErrorCodes.Auth.PasswordUppercaseMissing)
                .Matches("[a-z]").WithError(ErrorCodes.Auth.PasswordLowercaseMissing)
                .Matches("[0-9]").WithError(ErrorCodes.Auth.PasswordDigitMissing)
                .Matches(@"[\W_]").WithError(ErrorCodes.Auth.PasswordSpecialCharacterMissing)
                .Must(password => password == null || !password.Any(char.IsWhiteSpace)).WithError(ErrorCodes.Auth.PasswordWhitespaceInvalid);

            RuleFor(x => x.Email)
                .NotEmpty().WithError(ErrorCodes.User.EmailRequired)
                .EmailAddress().WithError(ErrorCodes.User.EmailInvalid)
                .MaximumLength(256).WithError(ErrorCodes.User.EmailTooLong)
                .MustAsync(async (email, cancellationToken) =>
                {
                    if (string.IsNullOrWhiteSpace(email)) return true;
                    return !await userRepository.IsEmailExist(EmailAddress.Create(email), cancellationToken);
                }).WithError(ErrorCodes.User.EmailAlreadyExists);

            When(x => !string.IsNullOrEmpty(x.PhoneNumber), () =>
            {
                RuleFor(x => x.PhoneNumber)
                   .NotEmpty().WithError(ErrorCodes.User.PhoneRequired)
                   .Must(phone => !string.IsNullOrWhiteSpace(phone)).WithError(ErrorCodes.User.PhoneRequired)
                   .Must(phone => phone == null || phone.Trim().Length <= 30).WithError(ErrorCodes.User.PhoneTooLong)
                   .Matches(@"^\+?[0-9\s\-\(\)]+$").WithError(ErrorCodes.User.PhoneInvalid);
            });

        }
    }
}
