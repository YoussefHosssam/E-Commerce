using E_Commerce.API.Common.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace E_Commerce.API.Common.Responses
{
    public class ApiResult<T> : IActionResult , IApiResult
    {
        public int StatusCode { get; set; }
        public ApiResponse<T> Response { get; set; } = default!;

        public bool IsSuccess => Response.IsSuccess;

        public object? ResponseObject => Response;

        public ApiError? Error => Response.Error;

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;
            response.StatusCode = StatusCode;
            response.ContentType = "application/json";

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(Response, options);
            await response.WriteAsync(json);
        }

        public static ApiResult<T> Success(T data, string msg, object? meta = null, int statusCode = 200)
        {
            return new ApiResult<T>
            {
                StatusCode = statusCode,
                Response = new ApiResponse<T>
                {
                    Data = data,
                    Message = msg,
                    Meta = meta,
                    IsSuccess = true,
                    Error = null
                }
            };
        }

        public static ApiResult<T> Fail(int statusCode, string errorCode, string errMsg)
        {
            return new ApiResult<T>
            {
                StatusCode = statusCode,
                Response = new ApiResponse<T>
                {
                    Data = default,
                    Message = null,
                    Meta = null,
                    Error = new ApiError
                    {
                        Code = errorCode,
                        Message = errMsg
                    },
                    IsSuccess = false
                }
            };
        }
    }

    public class ApiResult : IActionResult , IApiResult
    {
        public int StatusCode { get; set; }
        public ApiResponse Response { get; set; } = default!;

        public bool IsSuccess => Response.IsSuccess;

        public object? ResponseObject => Response;

        public ApiError? Error => Response.Error;

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;
            response.StatusCode = StatusCode;
            response.ContentType = "application/json";

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            options.Converters.Add(new JsonStringEnumConverter());
            var json = JsonSerializer.Serialize(Response, options);
            await response.WriteAsync(json);
        }

        public static ApiResult Success(string msg, object? meta = null, int statusCode = 200)
        {
            return new ApiResult
            {
                StatusCode = statusCode,
                Response = new ApiResponse
                {
                    Data = null,
                    Message = msg,
                    Meta = meta,
                    Error = null,
                    IsSuccess = true
                }
            };
        }

        public static ApiResult Fail(int statusCode, string errorCode, string errMsg)
        {
            return new ApiResult
            {
                StatusCode = statusCode,
                Response = new ApiResponse
                {
                    Data = null,
                    Message = null,
                    Meta = null,
                    Error = new ApiError
                    {
                        Code = errorCode,
                        Message = errMsg
                    },
                    IsSuccess = false
                }
            };
        }
    }
}