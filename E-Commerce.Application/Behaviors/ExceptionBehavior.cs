using E_Commerce.Application.Common.Errors;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace E_Commerce.Application.Behaviors
{
    public sealed class ExceptionBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<ExceptionBehavior<TRequest, TResponse>> _logger;

        public ExceptionBehavior(ILogger<ExceptionBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (DomainValidationException domainException)
            {
                _logger.LogWarning(
                    "Domain validation exception in {RequestName} with {ErrorCode}",
                    typeof(TRequest).Name,
                    domainException.Error.Code);

                return ResultFailureFactory.CreateFailure<TResponse>(domainException.Error);
            }
            catch (AppException appException)
            {
                _logger.LogError(
                    appException,
                    "Application exception in {RequestName} with {ErrorCode}",
                    typeof(TRequest).Name,
                    appException.Error.Code);

                return ResultFailureFactory.CreateFailure<TResponse>(appException.Error);
            }
        }
    }
}
