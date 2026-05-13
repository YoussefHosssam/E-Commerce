using E_Commerce.Application.Common.Entities;
using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Contracts.Services;

public interface IPasswordResetEmailPreparationService
{
    Task<PasswordResetEmailPreparationResult> PrepareAsync(User user, CancellationToken cancellationToken);
}
