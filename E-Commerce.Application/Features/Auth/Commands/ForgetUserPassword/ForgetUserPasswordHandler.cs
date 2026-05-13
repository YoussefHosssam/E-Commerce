using E_Commerce.Application.Common.Entities;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.BackgroundJobs;
using E_Commerce.Application.Contracts.Services;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;
using MediatR;

namespace E_Commerce.Application.Features.Auth.Commands.ForgetUserPassword;

public class ForgetUserPasswordHandler : IRequestHandler<ForgetUserPasswordCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly IEmailJobService _emailJobService;
    private readonly IPasswordResetEmailPreparationService _passwordResetEmailPreparationService;

    public ForgetUserPasswordHandler(
        IUnitOfWork uow,
        IEmailJobService emailJobService,
        IPasswordResetEmailPreparationService passwordResetEmailPreparationService)
    {
        _uow = uow;
        _emailJobService = emailJobService;
        _passwordResetEmailPreparationService = passwordResetEmailPreparationService;
    }

    public async Task<Result> Handle(ForgetUserPasswordCommand request, CancellationToken cancellationToken)
    {
        EmailAddress email = EmailAddress.Create(request.Email);
        User? user = await _uow.Users.GetUserByEmailAsync(email, cancellationToken);

        if (user is null)
            return Result.Success();

        AuthToken? lastResetToken = await _uow.AuthTokens.GetLastRelatedToken(
            user.Id,
            TokenType.ResetPasswordToken,
            cancellationToken);

        if (lastResetToken is null ||
            lastResetToken.IsTokenConsumed() ||
            lastResetToken.IsTokenExpired(DateTimeOffset.UtcNow))
        {
            PasswordResetEmailPreparationResult result = await _passwordResetEmailPreparationService.PrepareAsync(user, cancellationToken);
            await StoreAndQueueResetPasswordEmailAsync(result, cancellationToken);
            return Result.Success();
        }

        EmailMessage? emailMessage = await _uow.EmailMessages.GetSingleByPredicateAsync(
            em => em.RelatedTokenId == lastResetToken.Id &&
                  em.MessageType == MessageType.ResetPasswordMessage,
            cancellationToken);

        if (emailMessage is null)
        {
            PasswordResetEmailPreparationResult result = await _passwordResetEmailPreparationService.PrepareAsync(user, cancellationToken);
            await StoreAndQueueResetPasswordEmailAsync(result, cancellationToken);
            return Result.Success();
        }

        if (emailMessage.Status == EmailStatus.Sent)
        {
            lastResetToken.Consume();
            PasswordResetEmailPreparationResult result = await _passwordResetEmailPreparationService.PrepareAsync(user, cancellationToken);
            await StoreAndQueueResetPasswordEmailAsync(result, cancellationToken);
            return Result.Success();
        }

        _emailJobService.EnqueueResetPasswordEmail(emailMessage.Id, cancellationToken);
        return Result.Success();
    }

    private async Task StoreAndQueueResetPasswordEmailAsync(
        PasswordResetEmailPreparationResult result,
        CancellationToken cancellationToken)
    {
        await _uow.AuthTokens.CreateAsync(result.AuthToken, cancellationToken);
        await _uow.EmailMessages.CreateAsync(result.EmailMessage, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        _emailJobService.EnqueueResetPasswordEmail(result.EmailMessage.Id, cancellationToken);
    }
}
