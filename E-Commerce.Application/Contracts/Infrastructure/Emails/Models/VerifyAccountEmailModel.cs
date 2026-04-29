using E_Commerce.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Infrastructure.Emails.Models
{
    public record VerifyAccountEmailModel(string fullName , EmailAddress email , string token)
    {
        public string verificationUrl { get { return $"https//localhost/api/auth/v1/verify-email?token={token}"; } }
    }
}
