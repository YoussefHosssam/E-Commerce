using E_Commerce.Application.Contracts.Infrastructure.BackgroundJobs;
using E_Commerce.Application.Features.Auth.Commands.RegisterUser;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Infrastructure.BackgroundJobs.Jobs;
using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.BackgroundJobs.Hangfire
{
    public sealed class HangfireEmailQueueService : IEmailQueueService
    {
        private readonly IBackgroundJobClient _jobs;
        private readonly ILogger<HangfireEmailQueueService> _logger;

        public HangfireEmailQueueService(
            IBackgroundJobClient jobs,
            ILogger<HangfireEmailQueueService> logger)
        {
            _jobs = jobs;
            _logger = logger;
        }

        public string EnqueueVerificationEmail(Guid emailMessageId, CancellationToken cancellationToken)
        {
            var jobId = _jobs.Enqueue<EmailJobs>(
                x => x.SendVerificationEmailAsync(emailMessageId, cancellationToken));

            _logger.LogInformation(
                "Verification email job enqueued for EmailMessage {EmailMessageId} with BackgroundJob {BackgroundJobId}",
                emailMessageId,
                jobId);

            return jobId;
        }

        public string EnqueueResetPasswordEmail(Guid emailMessageId, CancellationToken cancellationToken)
        {
            var jobId = _jobs.Enqueue<EmailJobs>(
                x => x.SendResetPasswordEmailAsync(emailMessageId, cancellationToken));

            _logger.LogInformation(
                "Reset password email job enqueued for EmailMessage {EmailMessageId} with BackgroundJob {BackgroundJobId}",
                emailMessageId,
                jobId);

            return jobId;
        }
    }
}
