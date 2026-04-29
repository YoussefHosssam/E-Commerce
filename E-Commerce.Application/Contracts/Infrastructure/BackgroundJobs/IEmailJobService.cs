using E_Commerce.Application.Features.Auth.Commands.RegisterUser;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Infrastructure.BackgroundJobs
{
    public interface IEmailJobService
    {
        string EnqueueVerificationEmail(Guid emailMessageId, CancellationToken cancellationToken);
        //string EnqueueResetPasswordEmail(Guid userId, EmailAddress email, string token);
    }
}
