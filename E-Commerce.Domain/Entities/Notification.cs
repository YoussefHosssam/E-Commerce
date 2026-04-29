using E_Commerce.Domain.Common;

namespace E_Commerce.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    public string Type { get; set; } = default!; // "OrderPaid|OrderShipped|BackInStock|..."
    public string Title { get; set; } = default!;
    public string Body { get; set; } = default!;
    public string DataJson { get; set; } = "{}";
    public bool IsRead { get; set; }
    public DateTimeOffset? ReadAt { get; set; }
}
