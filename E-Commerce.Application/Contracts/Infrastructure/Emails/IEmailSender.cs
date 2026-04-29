using E_Commerce.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Infrastructure.Emails
{
    public interface IEmailSender
    {
        public Task SendAsync(EmailAddress recipient , string subject , string htmlBody, CancellationToken cancellationToken = default);
    }
}
