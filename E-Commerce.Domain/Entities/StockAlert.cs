using E_Commerce.Domain.Common;
using E_Commerce.Domain.Entities;


namespace E_Commerce.Domain.Entities;
public class StockAlert : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public Guid VariantId { get; set; }
    public Variant Variant { get; set; } = default!;

    public StockAlertStatus Status { get; set; } = StockAlertStatus.Active;

    public DateTimeOffset? TriggeredAt { get; set; }
}
