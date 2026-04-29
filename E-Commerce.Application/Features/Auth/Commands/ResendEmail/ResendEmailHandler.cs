using E_Commerce.Application.Common.Entities;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.BackgroundJobs;
using E_Commerce.Application.Contracts.Infrastructure.Common;
using E_Commerce.Application.Contracts.Persistence;
using E_Commerce.Application.Services.Contracts;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Auth.Commands.VerifyEmail
{
    public class ResendEmailHandler : IRequestHandler<ResendEmailCommand, Result>
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmailJobService _emailJobService;
        private readonly IVerificationEmailPreparationService _verificationEmailPreparationService;

        public ResendEmailHandler(
            IUnitOfWork uow,
            IEmailJobService emailJobService,
            IVerificationEmailPreparationService verificationEmailPreparationService,
            IAuthTokenRepository authTokenRepository)
        {
            _uow = uow;
            _emailJobService = emailJobService;
            _verificationEmailPreparationService = verificationEmailPreparationService;
        }
        public async Task<Result> Handle(ResendEmailCommand request, CancellationToken cancellationToken)
        {
            EmailAddress email = EmailAddress.Create(request.Email);
            User? user = await _uow.Users.GetSingleByPredicateAsync(x => x.Email == email, cancellationToken);
            if (user is null) return Result.Success();
            if (user.VerificationStatus == VerificationStatus.Verified) return Result.Success();
            AuthToken? lastVerificationRelatedToken = await _uow.AuthTokens.GetLastRelatedToken(user.Id, TokenType.VerifyEmailToken, cancellationToken);
            if (lastVerificationRelatedToken is null)
            {
                VerificationEmailPreparationResult result = await _verificationEmailPreparationService.PrepareAsync(user, cancellationToken);
                await StoreAndQueueVerificationEmailAsync(result, cancellationToken);
                return Result.Success();
            }
            if (lastVerificationRelatedToken.ConsumedAt.HasValue ||
                lastVerificationRelatedToken.IsTokenExpired(DateTimeOffset.UtcNow))
            {
                VerificationEmailPreparationResult result = await _verificationEmailPreparationService.PrepareAsync(user, cancellationToken);
                await StoreAndQueueVerificationEmailAsync(result, cancellationToken);
                return Result.Success();
            }
            EmailMessage? emailMessage = await _uow.EmailMessages.GetSingleByPredicateAsync(em => em.RelatedTokenId == lastVerificationRelatedToken.Id && em.MessageType == MessageType.VerifyEmailMessage , cancellationToken);
            if (emailMessage is null)
            {
                VerificationEmailPreparationResult result = await _verificationEmailPreparationService.PrepareAsync(user, cancellationToken);
                await StoreAndQueueVerificationEmailAsync(result, cancellationToken);
                return Result.Success();
            }
            _emailJobService.EnqueueVerificationEmail(emailMessage.Id, cancellationToken);
            return Result.Success();
        }
        private async Task StoreAndQueueVerificationEmailAsync(VerificationEmailPreparationResult result , CancellationToken cancellationToken)
        {
            await _uow.AuthTokens.CreateAsync(result.AuthToken, cancellationToken);
            await _uow.EmailMessages.CreateAsync(result.EmailMessage, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            _emailJobService.EnqueueVerificationEmail(result.EmailMessage.Id, cancellationToken);
        }
    }
}
