using System.Text.RegularExpressions;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Exceptions;

namespace E_Commerce.Domain.ValueObjects;

public sealed class Color : IEquatable<Color>
{
    private const int NameMaxLength = 30;

    private static readonly Regex HexCodeRegex = new(
        "^#[0-9A-F]{6}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    public string Name { get; private set; } = default!;
    public string HexCode { get; private set; } = default!;

    private Color() { } // EF

    private Color(string name, string hexCode)
    {
        Name = name;
        HexCode = hexCode;
    }

    public static Color Create(string name, string hexCode)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException(VariantErrors.ColorNameRequired);

        if (string.IsNullOrWhiteSpace(hexCode))
            throw new DomainValidationException(VariantErrors.ColorHexCodeRequired);

        var normalizedName = name.Trim();
        if (normalizedName.Length > NameMaxLength)
            throw new DomainValidationException(VariantErrors.ColorTooLong);

        var normalizedHexCode = hexCode.Trim().ToUpperInvariant();
        if (!HexCodeRegex.IsMatch(normalizedHexCode))
            throw new DomainValidationException(VariantErrors.ColorHexCodeInvalid);

        return new Color(normalizedName, normalizedHexCode);
    }

    public bool Equals(Color? other)
    {
        return other is not null
            && string.Equals(HexCode, other.HexCode, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => Equals(obj as Color);

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(HexCode);

    public static bool operator ==(Color? left, Color? right) => Equals(left, right);

    public static bool operator !=(Color? left, Color? right) => !Equals(left, right);
}
