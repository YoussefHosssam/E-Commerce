using E_Commerce.Domain.Common.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Common.Result
{
    public class Result
    {
        public bool IsSuccess { get; }
        public Error? Error { get; }
        public object? Meta { get; }

        protected Result(bool isSuccess, Error? error, object? meta = null)
        {
            IsSuccess = isSuccess;
            Error = error;
            Meta = meta;
        }

        public static Result Success(object? Meta = null) => new Result(true, null , Meta);

        public static Result Fail(Error error) => new Result(false, error);
    }

    public class Result<T> : Result
    {
        public T? Data { get; }
        private Result(T data , object? meta) : base(true, null , meta)
        {
            Data = data;
        }

        private Result(Error error) : base(false, error)
        {
            Data = default;
        }

        public static Result<T> Success(T data , object? Meta = null) => new Result<T>(data , Meta);

        public static new Result<T> Fail(Error error) => new Result<T>(error);
    }
}