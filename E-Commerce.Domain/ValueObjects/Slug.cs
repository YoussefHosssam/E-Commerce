using System.Text.RegularExpressions;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Exceptions;

namespace E_Commerce.Domain.ValueObjects;

public readonly record struct Slug
{
    private static readonly Regex Rx = new(
        @"^[a-z0-9]+(?:-[a-z0-9]+)*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private Slug(string value) => Value = value;

    public static Slug Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainValidationException(ErrorCodes.ValueObjects.SlugRequired);

        var normalized = value.Trim().ToLowerInvariant();

        normalized = normalized.Replace('_', '-');
        normalized = Regex.Replace(normalized, @"\s+", "-");

        if (normalized.Length > 150)
            throw new DomainValidationException(ErrorCodes.ValueObjects.SlugInvalid);

        if (!Rx.IsMatch(normalized))
            throw new DomainValidationException(ErrorCodes.ValueObjects.SlugInvalid);

        return new Slug(normalized);
    }

    public override string ToString() => Value;
}