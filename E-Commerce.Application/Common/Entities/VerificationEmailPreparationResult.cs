using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Common.Entities
{
    public sealed class VerificationEmailPreparationResult
    {
        public VerificationEmailPreparationResult(AuthToken authToken, EmailMessage emailMessage)
        {
            AuthToken = authToken;
            EmailMessage = emailMessage;
        }

        public AuthToken AuthToken { get; }
        public EmailMessage EmailMessage { get; }
    }
}
