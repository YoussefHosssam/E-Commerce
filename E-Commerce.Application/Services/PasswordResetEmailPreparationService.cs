using E_Commerce.Application.Common.Entities;
using E_Commerce.Application.Contracts.Infrastructure.Common;
using E_Commerce.Application.Contracts.Infrastructure.Emails;
using E_Commerce.Application.Contracts.Infrastructure.Emails.Models;
using E_Commerce.Application.Contracts.Services;
using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Application.Services;

public sealed class PasswordResetEmailPreparationService : IPasswordResetEmailPreparationService
{
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ITokenHasher _tokenHasher;
    private readonly IEmailTemplateRenderer _emailTemplateRenderer;

    public PasswordResetEmailPreparationService(
        ITokenGenerator tokenGenerator,
        ITokenHasher tokenHasher,
        IEmailTemplateRenderer emailTemplateRenderer)
    {
        _tokenGenerator = tokenGenerator;
        _tokenHasher = tokenHasher;
        _emailTemplateRenderer = emailTemplateRenderer;
    }

    public async Task<PasswordResetEmailPreparationResult> PrepareAsync(User user, CancellationToken cancellationToken)
    {
        var rawToken = await _tokenGenerator.GenerateTokenAsync();
        TokenHash tokenHash = await _tokenHasher.HashAsync(rawToken, cancellationToken);

        var authToken = AuthToken.Create(
            user.Id,
            TokenType.ResetPasswordToken,
            tokenHash,
            DateTimeOffset.UtcNow.AddMinutes(30));

        var model = new ResetPasswordEmailModel(
            user.FullName,
            user.Email,
            rawToken);

        var htmlBody = await _emailTemplateRenderer.RenderResetPasswordAsync(model, cancellationToken);

        var emailMessage = EmailMessage.Create(
            user.Id,
            authToken.Id,
            MessageType.ResetPasswordMessage,
            user.Email,
            EmailSubject.ResetPasswordSubject.GetDescription(),
            htmlBody);

        return new PasswordResetEmailPreparationResult(authToken, emailMessage);
    }
}
