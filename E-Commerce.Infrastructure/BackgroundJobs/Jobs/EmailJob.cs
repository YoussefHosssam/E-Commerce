using E_Commerce.Application.Contracts.Infrastructure.Emails;
using E_Commerce.Application.Features.Auth.Commands.RegisterUser;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;
using Hangfire;
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
        public EmailJobs(IEmailSender emailSender, IUnitOfWork uow)
        {
            _emailSender = emailSender;
            _uow = uow;
        }

        [AutomaticRetry(Attempts = 5)]
        public async Task SendVerificationEmailAsync(Guid emailMessageId, CancellationToken cancellationToken)
        {
            EmailMessage? emailMessage = await _uow.EmailMessages.GetByIdAsync(emailMessageId, cancellationToken);
            if (emailMessage is null) return;
            if (emailMessage.Status == EmailStatus.Sent) return;
            emailMessage.MarkAttempt();
            await _uow.SaveChangesAsync(cancellationToken);
            try
            {
                await _emailSender.SendAsync(emailMessage.Recipient, emailMessage.Subject, emailMessage.BodyHtml, cancellationToken);
                emailMessage.MarkAsSent("MailTrap");
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                emailMessage.MarkAsFailed("MailTrap");
                await _uow.SaveChangesAsync(cancellationToken);
                throw;
            }
        }

        //public async Task SendResetPasswordEmailAsync(Guid userId, EmailAddress email, string token)
        //{
        //    await _emailSender.SendAsync(email, "Reset Password", $"Token: {token}");
        //}
    }
}
