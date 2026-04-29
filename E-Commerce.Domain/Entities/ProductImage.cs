using E_Commerce.Domain.Common;
using E_Commerce.Domain.Entities;


namespace E_Commerce.Domain.Entities;
public class ProductImage : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;
    public string Url { get; set; } = default!;
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
}

