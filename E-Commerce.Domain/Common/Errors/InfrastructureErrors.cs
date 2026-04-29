namespace E_Commerce.Domain.Common.Errors;

public static class InfrastructureErrors
{
    public static readonly Error PersistenceFailure = new(ErrorCodes.Infrastructure.PersistenceFailure, "A persistence error occurred.", ErrorType.Failure);
    public static readonly Error DatabaseUnavailable = new(ErrorCodes.Infrastructure.DatabaseUnavailable, "The database is temporarily unavailable.", ErrorType.Failure);
}
