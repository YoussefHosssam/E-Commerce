using E_Commerce.Domain.Common;
namespace E_Commerce.Domain.Entities;

public class Favorite : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    public Guid? VariantId { get; set; }
    public Variant? Variant { get; set; }
}
