using E_Commerce.Domain.Exceptions;
using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;

namespace E_Commerce.Domain.Entities;
public class VariantImage : BaseEntity
{
    public Guid VariantId { get; private set; }
    public string Url { get; private set; } = default!;
    public bool IsPrimary { get; private set; } = true;
    public int SortOrder { get; private set; } = 1;
    private VariantImage(Guid variantId, string url, bool isPrimary, int sortOrder)
    {
        VariantId = variantId;
        Url = url;
        IsPrimary = isPrimary;
        SortOrder = sortOrder;
    }
    private VariantImage(){  }
    public static VariantImage Create(Guid variantId, string url, bool isPrimary, int sortOrder)
    {
        if (variantId == Guid.Empty) throw new DomainValidationException(VariantImageErrors.VariantIdEmpty);
        if (string.IsNullOrEmpty(url)) throw new DomainValidationException(VariantImageErrors.UrlEmpty);
        if (sortOrder <= 0) throw new DomainValidationException(VariantImageErrors.SortOrderInvalid);
        return new(variantId, url, isPrimary, sortOrder);
    }
}


