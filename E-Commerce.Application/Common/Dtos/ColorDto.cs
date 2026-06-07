using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Application.Common.Dtos;

public sealed record ColorDto(string Name, string HexCode)
{
    public static ColorDto FromColor(Color color) => new(color.Name, color.HexCode);
}
