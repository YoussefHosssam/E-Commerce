using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Exceptions;

namespace E_Commerce.Domain.ValueObjects;

public readonly record struct TokenHash : IEquatable<string>
{
    public string Value { get; }

    private TokenHash(string value) => Value = value;

    public static TokenHash Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainValidationException(ValueObjectErrors.TokenHashRequired);

        value = value.Trim();

        if (value.Length < 20)
            throw new DomainValidationException(ValueObjectErrors.TokenHashInvalid);

        return new TokenHash(value);
    }

    public override string ToString() => Value;
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public bool Equals(string? other)
    {
        return Value.ToString() == other?.ToString();
    }
}
