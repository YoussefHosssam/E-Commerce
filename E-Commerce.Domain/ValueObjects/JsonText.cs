using E_Commerce.Domain.Common.Errors;
using System.Text.Json;

public sealed record JsonText
{
    public string Value { get; }

    private JsonText(string value)
    {
        Value = value;
    }

    public static JsonText Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            value = "{}";

        ValidateJson(value);

        return new JsonText(value.Trim());
    }

    public static JsonText From<T>(T obj)
    {
        if (obj is null)
            return new JsonText("{}");

        var json = JsonSerializer.Serialize(obj);

        return Create(json);
    }

    public static T To<T>(JsonText text)
    {
        if (text is null)
            throw new DomainValidationException(ValueObjectErrors.JsonInvalid);

        var result = JsonSerializer.Deserialize<T>(text.Value);

        if (result is null)
            throw new DomainValidationException(ValueObjectErrors.JsonInvalid);

        return result;
    }

    private static void ValidateJson(string value)
    {
        try
        {
            using var _ = JsonDocument.Parse(value);
        }
        catch (JsonException ex)
        {
            throw new DomainValidationException(ValueObjectErrors.JsonInvalid);
        }
    }

    public override string ToString() => Value;
}