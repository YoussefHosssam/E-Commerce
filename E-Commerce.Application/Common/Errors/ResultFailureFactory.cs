using AppResult = E_Commerce.Application.Common.Result.Result;
using AppGenericResult = E_Commerce.Application.Common.Result.Result<int>;
using E_Commerce.Domain.Common.Errors;

namespace E_Commerce.Application.Common.Errors;

internal static class ResultFailureFactory
{
    public static TResponse CreateFailure<TResponse>(Error error)
    {
        var responseType = typeof(TResponse);

        if (responseType == typeof(AppResult))
        {
            return (TResponse)(object)AppResult.Fail(error);
        }

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(E_Commerce.Application.Common.Result.Result<>))
        {
            var valueType = responseType.GetGenericArguments()[0];
            var failMethod = typeof(E_Commerce.Application.Common.Result.Result<>)
                .MakeGenericType(valueType)
                .GetMethod(nameof(AppGenericResult.Fail), new[] { typeof(Error) });

            if (failMethod is null)
            {
                throw new InvalidOperationException("Result<T>.Fail(Error) not found.");
            }

            return (TResponse)failMethod.Invoke(null, new object[] { error })!;
        }

        throw new InvalidOperationException($"Pipeline requires response type Result or Result<T>. Current: {responseType.Name}");
    }
}
