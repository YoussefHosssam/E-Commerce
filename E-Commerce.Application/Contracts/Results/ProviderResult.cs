using E_Commerce.Domain.Common.Errors;

namespace E_Commerce.Application.Contracts.Results;

public sealed class ProviderResult<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Data { get; }
    public Error? Error { get; }
    public string RawPayloadJson { get; }

    private ProviderResult(
        bool isSuccess,
        T? data,
        Error? error,
        string rawPayloadJson)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
        RawPayloadJson = rawPayloadJson;
    }

    public static ProviderResult<T> Success(T data, string rawPayloadJson)
    {
        return new ProviderResult<T>(
            true,
            data,
            null,
            rawPayloadJson);
    }

    public static ProviderResult<T> Fail(Error error, string rawPayloadJson = "{}")
    {
        return new ProviderResult<T>(
            false,
            default,
            error,
            rawPayloadJson);
    }
}