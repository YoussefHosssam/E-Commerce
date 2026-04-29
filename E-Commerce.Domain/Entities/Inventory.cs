

namespace E_Commerce.Domain.Entities;

public class Inventory
{
    public Guid VariantId { get; set; }
    public Variant Variant { get; set; } = default!;
    public int OnHand { get; set; }
    public int Reserved { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}

