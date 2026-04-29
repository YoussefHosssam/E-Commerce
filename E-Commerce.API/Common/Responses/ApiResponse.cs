namespace E_Commerce.API.Common.Responses
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public string? Message { get; set; }
        public ApiError? Error { get; set; }
        public object? Meta { get; set; }
        public bool IsSuccess { get; set; } = true;
    }

    public class ApiResponse
    {
        public object? Data { get; set; }
        public string? Message { get; set; }
        public ApiError? Error { get; set; }
        public object? Meta { get; set; }
        public bool IsSuccess { get; set; } = true;
    }

    public class ApiError
    {
        public string Code { get; set; } = default!;
        public string Message { get; set; } = default!;
    }
}