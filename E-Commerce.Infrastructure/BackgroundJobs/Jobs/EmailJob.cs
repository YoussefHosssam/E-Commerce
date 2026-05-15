using E_Commerce.Application.Contracts.Infrastructure.Emails;
using E_Commerce.Application.Features.Auth.Commands.RegisterUser;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;
using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.BackgroundJobs.Jobs
{
    public sealed class EmailJobs
    {
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<EmailJobs> _logger;

        public EmailJobs(
            IEmailSender emailSender,
            IUnitOfWork uow,
            ILogger<EmailJobs> logger)
        {
            _emailSender = emailSender;
            _uow = uow;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 5)]
        public async Task SendVerificationEmailAsync(Guid emailMessageId, CancellationToken cancellationToken)
        {
            EmailMessage? emailMessage = await _uow.EmailMessages.GetByIdAsync(emailMessageId, cancellationToken);
            if (emailMessage is null) return;
            if (emailMessage.Status == EmailStatus.Sent) return;

            _logger.LogInformation(
                "Verification email job started for EmailMessage {EmailMessageId}",
                emailMessageId);

            emailMessage.MarkAttempt();
            await _uow.SaveChangesAsync(cancellationToken);
            try
            {
                await _emailSender.SendAsync(emailMessage.Recipient, emailMessage.Subject, emailMessage.BodyHtml, cancellationToken);
                emailMessage.MarkAsSent("MailTrap");
                await _uow.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Verification email job completed for EmailMessage {EmailMessageId}",
                    emailMessageId);
            }
            catch (Exception exception)
            {
                emailMessage.MarkAsFailed("MailTrap");
                await _uow.SaveChangesAsync(cancellationToken);

                _logger.LogError(
                    exception,
                    "Verification email job failed for EmailMessage {EmailMessageId}",
                    emailMessageId);

                throw;
            }
        }

        [AutomaticRetry(Attempts = 5)]
        public async Task SendResetPasswordEmailAsync(Guid emailMessageId, CancellationToken cancellationToken)
        {
            EmailMessage? emailMessage = await _uow.EmailMessages.GetByIdAsync(emailMessageId, cancellationToken);
            if (emailMessage is null) return;
            if (emailMessage.Status == EmailStatus.Sent) return;
            if (emailMessage.MessageType != MessageType.ResetPasswordMessage) return;

            _logger.LogInformation(
                "Reset password email job started for EmailMessage {EmailMessageId}",
                emailMessageId);

            emailMessage.MarkAttempt();
            await _uow.SaveChangesAsync(cancellationToken);

            try
            {
                await _emailSender.SendAsync(emailMessage.Recipient, emailMessage.Subject, emailMessage.BodyHtml, cancellationToken);
                emailMessage.MarkAsSent("MailTrap");
                await _uow.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Reset password email job completed for EmailMessage {EmailMessageId}",
                    emailMessageId);
            }
            catch (Exception exception)
            {
                emailMessage.MarkAsFailed("MailTrap");
                await _uow.SaveChangesAsync(cancellationToken);

                _logger.LogError(
                    exception,
                    "Reset password email job failed for EmailMessage {EmailMessageId}",
                    emailMessageId);

                throw;
            }
        }
    }
}
