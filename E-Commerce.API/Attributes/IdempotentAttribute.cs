using E_Commerce.API.Filters;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class IdempotentAttribute : TypeFilterAttribute
    {
        public IdempotentAttribute(string operation) : base(typeof(IdempotencyFilter))
        {
            Operation = operation;
        }
        public string Operation { get; }
        public int ExpirationMinutes { get; init; } = 30;
    }
}
