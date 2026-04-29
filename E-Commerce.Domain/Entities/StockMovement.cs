using E_Commerce.Domain.Common;


namespace E_Commerce.Domain.Entities;
public class StockMovement : BaseEntity
{
    public Guid VariantId { get; set; }
    public Variant Variant { get; set; } = default!;
    public StockMovementType Type { get; set; }
    public int QuantityDelta { get; set; }
    public string? Reason { get; set; }
    public Guid? RefId { get; set; }
    public Guid? ActorUserId { get; set; }
    public User? ActorUser { get; set; }
}

