using E_Commerce.Domain.Common.Errors;

namespace E_Commerce.Domain.Exceptions;

public sealed class DomainValidationException : AppException
{
    public DomainValidationException(Error error)
        : base(error)
    {
    }
}
