using E_Commerce.Application.Contracts.Infrastructure.Emails;
using E_Commerce.Application.Contracts.Infrastructure.Emails.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Emails
{
    public class EmailTemplateRenderer : IEmailTemplateRenderer
    {
        public Task<string> RenderVerifyAccountAsync(VerifyAccountEmailModel model, CancellationToken cancellationToken = default)
        {
            string html = $"""
        <html>
            <body>
                <h1>Verify your account</h1>
                <p>Hello {model.fullName},</p>
                <p>Please verify your account by clicking the link below:</p>
                <a href="{model.verificationUrl}">Verify Account</a>
            </body>
        </html>
        """;

            return Task.FromResult(html);
        }
    }
}
