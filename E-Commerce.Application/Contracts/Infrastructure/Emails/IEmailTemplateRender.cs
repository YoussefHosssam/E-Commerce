using E_Commerce.Application.Contracts.Infrastructure.Emails.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Infrastructure.Emails
{
    public interface IEmailTemplateRenderer
    {
        Task<string> RenderVerifyAccountAsync(VerifyAccountEmailModel model, CancellationToken cancellationToken = default);
    }
}
