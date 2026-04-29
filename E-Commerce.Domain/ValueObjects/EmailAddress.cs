using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace E_Commerce.Domain.ValueObjects;

public readonly record struct EmailAddress
{
    private static readonly Regex Rx = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private EmailAddress(string normalized)
        => Value = normalized;

    public static EmailAddress Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainValidationException(ErrorCodes.ValueObjects.EmailRequired);

        var normalized = value.Trim().ToLowerInvariant();

        if (!Rx.IsMatch(normalized))
            throw new DomainValidationException(ErrorCodes.ValueObjects.EmailInvalid);

        return new EmailAddress(normalized);
    }

    public override string ToString() => Value;
}
