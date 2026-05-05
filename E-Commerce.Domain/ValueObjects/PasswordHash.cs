
using E_Commerce.Domain.Common.Errors;

namespace E_Commerce.Domain.ValueObjects;

public readonly record struct PasswordHash
{
    public readonly string Value { get;}
    private PasswordHash(string value)
    {
        Value = value;
    }
    public static PasswordHash Create(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new DomainValidationException(ValueObjectErrors.PasswordHashInvalid);
        }
        return new PasswordHash(value);
    }
}

