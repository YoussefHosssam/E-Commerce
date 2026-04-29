using E_Commerce.Application.Common.Entities;
using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.Contracts
{
    public interface IVerificationEmailPreparationService
    {
        Task<VerificationEmailPreparationResult> PrepareAsync(User user, CancellationToken cancellationToken);
    }
}
