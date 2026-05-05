
namespace E_Commerce.Domain.ValueObjects;
public readonly record struct JsonText
{
    public string Value { get; }
    private JsonText(string value) => Value = value;

    public static JsonText Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            value = "{}";

        return new JsonText(value.Trim());
    }

    public override string ToString() => Value;
}