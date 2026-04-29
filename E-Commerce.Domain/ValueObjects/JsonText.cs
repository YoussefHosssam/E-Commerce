
namespace E_Commerce.Domain.ValueObjects;
public readonly record struct JsonText
{
    public string Value { get; }
    private JsonText(string value) => Value = value;

    public static JsonText Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            value = "{}";

        // Optional: حاول تتحقق إنه JSON valid (System.Text.Json) - ده ممكن يبقى heavy
        return new JsonText(value.Trim());
    }

    public override string ToString() => Value;
}