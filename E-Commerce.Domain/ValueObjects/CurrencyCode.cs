using System.Text.RegularExpressions;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Exceptions;

namespace E_Commerce.Domain.ValueObjects;

public readonly record struct CurrencyCode
{
    private static readonly Regex Rx = new(
        @"^[A-Z]{3}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private CurrencyCode(string value) => Value = value;

    public static CurrencyCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainValidationException(ErrorCodes.ValueObjects.CurrencyRequired);

        var normalized = value.Trim().ToUpperInvariant();

        if (!Rx.IsMatch(normalized))
            throw new DomainValidationException(ErrorCodes.ValueObjects.CurrencyInvalid);

        return new CurrencyCode(normalized);
    }

    public override string ToString() => Value;
}
