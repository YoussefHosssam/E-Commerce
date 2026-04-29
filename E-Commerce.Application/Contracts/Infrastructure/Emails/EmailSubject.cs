using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Infrastructure.Emails
{
    internal enum EmailSubject
    {
        [Description("Verify your account")]
        VerifyAccountSubject,
        [Description("Reset your password")]
        ResetPasswordSubject
    }
}
