namespace E_Commerce.Domain.Common.Errors;

public class AppException : Exception
{
    public Error Error { get; }

    public AppException(Error error, Exception? innerException = null)
        : base(error.Message, innerException)
    {
        Error = error;
    }
}
