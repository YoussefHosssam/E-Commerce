using E_Commerce.Application.Common.Entities;
using E_Commerce.Application.Contracts.Infrastructure.Common;
using E_Commerce.Application.Contracts.Infrastructure.Emails;
using E_Commerce.Application.Contracts.Infrastructure.Emails.Models;
using E_Commerce.Application.Extensions;
using E_Commerce.Application.Services.Contracts;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class VerificationEmailPreparationService : IVerificationEmailPreparationService
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ITokenHasher _tokenHasher;
        private readonly IEmailTemplateRenderer _emailTemplateRenderer;

        public VerificationEmailPreparationService(
            ITokenGenerator tokenGenerator,
            ITokenHasher tokenHasher,
            IEmailTemplateRenderer emailTemplateRenderer)
        {
            _tokenGenerator = tokenGenerator;
            _tokenHasher = tokenHasher;
            _emailTemplateRenderer = emailTemplateRenderer;
        }

        public async Task<VerificationEmailPreparationResult> PrepareAsync(User user, CancellationToken cancellationToken)
        {
            string rawToken = await _tokenGenerator.GenerateTokenAsync();

            TokenHash tokenHash = await _tokenHasher.HashAsync(rawToken , cancellationToken);

            AuthToken authToken = AuthToken.Create(
                user.Id,
                TokenType.VerifyEmailToken,
                tokenHash,
                DateTimeOffset.UtcNow.AddHours(24));

            VerifyAccountEmailModel model = new(
                user.FullName,
                user.Email,
                rawToken);

            string htmlBody = await _emailTemplateRenderer.RenderVerifyAccountAsync(model, cancellationToken);

            EmailMessage emailMessage = EmailMessage.Create(
                user.Id,
                authToken.Id,
                MessageType.VerifyEmailMessage,
                user.Email,
                EmailSubject.VerifyAccountSubject.GetDescription(),
                htmlBody);

            return new VerificationEmailPreparationResult(authToken, emailMessage);
        }
    }
}
