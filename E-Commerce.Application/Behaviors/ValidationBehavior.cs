using E_Commerce.Application.Common.Errors;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace E_Commerce.Application.Behaviors
{
    internal class ValidationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

        public ValidationBehavior(
            IEnumerable<IValidator<TRequest>> validators,
            ILogger<ValidationBehavior<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken))
                );

                var failure = validationResults
                    .SelectMany(r => r.Errors)
                    .FirstOrDefault(f => f is not null);

                if (failure is not null)
                {
                    var error = new Error(failure.ErrorCode, failure.ErrorMessage, ErrorType.Validation);

                    _logger.LogWarning(
                        "Validation failed for {RequestName} with {ErrorCode} on {PropertyName}",
                        typeof(TRequest).Name,
                        failure.ErrorCode,
                        failure.PropertyName);

                    return ResultFailureFactory.CreateFailure<TResponse>(error);
                }
            }

            return await next();
        }
    }
}
