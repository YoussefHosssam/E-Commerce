using E_Commerce.Application.Contracts.Infrastructure.BackgroundJobs;
using E_Commerce.Application.Features.Auth.Commands.RegisterUser;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Infrastructure.BackgroundJobs.Jobs;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.BackgroundJobs.Hangfire
{
    public sealed class HangfireEmailJobService : IEmailJobService
    {
        private readonly IBackgroundJobClient _jobs;

        public HangfireEmailJobService(IBackgroundJobClient jobs)
        {
            _jobs = jobs;
        }

        public string EnqueueVerificationEmail(Guid emailMessageId, CancellationToken cancellationToken)
        {
            return _jobs.Enqueue<EmailJobs>(
                x => x.SendVerificationEmailAsync(emailMessageId, cancellationToken));
        }

        //public string EnqueueResetPasswordEmail(Guid userId, EmailAddress email, string token)
        //{
        //    return _jobs.Enqueue<EmailJobs>(
        //        x => x.SendResetPasswordEmailAsync(userId, email, token));
        //}
    }
}
