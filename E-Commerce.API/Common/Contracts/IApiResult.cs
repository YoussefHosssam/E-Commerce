using E_Commerce.API.Common.Responses;

namespace E_Commerce.API.Common.Contracts
{
    public interface IApiResult
    {
        int StatusCode { get; }
        bool IsSuccess { get; }
        object? ResponseObject { get; }
        ApiError? Error { get; }
    }
}
