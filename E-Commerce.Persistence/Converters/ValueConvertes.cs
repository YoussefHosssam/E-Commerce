using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace E_Commerce.Infrastructure.Persistence.Converters;

internal static class ValueConverters
{
    public static ValueConverter<T, string> StructString<T>() where T : struct
        => new(v => (string)typeof(T).GetProperty("Value")!.GetValue(v)!,
               s => ValueObjectFactory.FromString<T>(s));
}