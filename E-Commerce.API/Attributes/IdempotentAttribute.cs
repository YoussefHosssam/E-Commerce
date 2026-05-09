namespace E_Commerce.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class IdempotentAttribute : Attribute
    {
        public IdempotentAttribute(string operation)
        {
            Operation = operation;
        }
        public string Operation { get; }
        public int ExpirationMinutes { get; init; } = 30;
    }
}
