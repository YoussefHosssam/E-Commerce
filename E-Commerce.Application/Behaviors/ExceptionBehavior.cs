using E_Commerce.Application.Common.Errors;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Exceptions;
using MediatR;

namespace E_Commerce.Application.Behaviors
{
    public sealed class ExceptionBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
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
                return ResultFailureFactory.CreateFailure<TResponse>(domainException.Error);
            }
            catch (AppException appException)
            {
                return ResultFailureFactory.CreateFailure<TResponse>(appException.Error);
            }
        }
    }
}
