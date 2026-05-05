using E_Commerce.Application.Common.Result;
using AutoMapper;
using E_Commerce.Application.Common.Entities;
using E_Commerce.Application.Contracts.Infrastructure.BackgroundJobs;
using E_Commerce.Application.Contracts.Infrastructure.Common;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using MediatR;
using E_Commerce.Application.Contracts.Services;

namespace E_Commerce.Application.Features.Auth.Commands.RegisterUser
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasherAdapter _passwordHasher;
        private readonly IVerificationEmailPreparationService _verificationEmailPreparationService;
        private readonly IEmailJobService _emailJobService;
        private readonly IMapper _mapper;

        public RegisterUserHandler(IUnitOfWork uow, IPasswordHasherAdapter passwordHasher, IEmailJobService emailJobService, IMapper mapper, IVerificationEmailPreparationService verificationEmailPreparationService)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _emailJobService = emailJobService;
            _mapper = mapper;
            _verificationEmailPreparationService = verificationEmailPreparationService;
        }

        async Task<Result> IRequestHandler<RegisterUserCommand, Result>.Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            PasswordHash hashedPassword = _passwordHasher.Hash(request.Password);
            User user = User.Create(request.Email, hashedPassword, request.FirstName, request.LastName, request.PhoneNumber);
            VerificationEmailPreparationResult result = await _verificationEmailPreparationService.PrepareAsync(user, cancellationToken);
            await _uow.Users.CreateAsync(user, cancellationToken);
            await _uow.AuthTokens.CreateAsync(result.AuthToken, cancellationToken);
            await _uow.EmailMessages.CreateAsync(result.EmailMessage, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            _emailJobService.EnqueueVerificationEmail(result.EmailMessage.Id, cancellationToken);
            return Result.Success();
        }
    }
}
