using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Common.Entities;

public sealed class PasswordResetEmailPreparationResult
{
    public PasswordResetEmailPreparationResult(AuthToken authToken, EmailMessage emailMessage)
    {
        AuthToken = authToken;
        EmailMessage = emailMessage;
    }

    public AuthToken AuthToken { get; }
    public EmailMessage EmailMessage { get; }
}
