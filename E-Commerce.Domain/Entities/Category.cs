using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Exceptions;
using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Domain.Entities;

public sealed class Category : BaseEntity
{
    public Guid? ParentId { get; private set; }
    public Category? Parent { get; private set; } // EF navigation

    private readonly List<Category> _children = new();
    public IReadOnlyCollection<Category> Children => _children.AsReadOnly();

    public Slug Slug { get; private set; } = default!; // unique (DB unique index)
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Usually not part of category aggregate (query navigation)
    private readonly List<Product> _products = new();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Category() { } // EF

    private Category(Guid? parentId, Slug slug, int sortOrder)
    {
        ParentId = parentId;
        Slug = slug;
        SortOrder = sortOrder;
        IsActive = true;
    }

    public static Category Create(Guid? parentId, Slug slug, int sortOrder = 0)
    {
        if (parentId.HasValue && parentId.Value == Guid.Empty)
            throw new DomainValidationException(ErrorCodes.Category.ParentInvalid);

        if (slug.Equals(default(Slug)))
            throw new DomainValidationException(ErrorCodes.Category.SlugRequired);

        if (sortOrder < 0)
            throw new DomainValidationException(ErrorCodes.Category.SortOrderInvalid);

        return new Category(parentId, slug, sortOrder);
    }

    // --------- Behaviors ---------

    public void ChangeSlug(Slug slug)
    {
        if (slug.Equals(default(Slug)))
            throw new DomainValidationException(ErrorCodes.Category.SlugRequired);

        // uniqueness globally is DB/application responsibility
        Slug = slug;
    }

    public void SetSortOrder(int sortOrder)
    {
        if (sortOrder < 0)
            throw new DomainValidationException(ErrorCodes.Category.SortOrderInvalid);

        SortOrder = sortOrder;
    }

    public void ChangeParent(Category? parent)
    {
        if (parent is null)
        {
            ClearParent();
            return;
        }

        SetParent(parent);
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Attach child category under this category.
    /// Only local validation here (self-parent).
    /// Cycle prevention beyond this requires repository check in application layer.
    /// </summary>
    public void AddChild(Category child)
    {
        if (child is null)
            throw new DomainValidationException(ErrorCodes.Category.ChildRequired);

        if (child.Id == this.Id)
            throw new DomainValidationException(ErrorCodes.Domain.Category.ChildSelf);

        // prevent duplicate child
        if (_children.Any(c => c.Id == child.Id))
            throw new DomainValidationException(ErrorCodes.Category.ChildDuplicate);

        // set parent link (domain owns the relationship)
        child.SetParent(this);

        _children.Add(child);
    }

    public void RemoveChild(Guid childId)
    {
        var idx = _children.FindIndex(c => c.Id == childId);
        if (idx < 0)
            throw new DomainValidationException(ErrorCodes.Domain.Category.ChildNotFound);

        _children[idx].ClearParent();
        _children.RemoveAt(idx);
    }

    private void SetParent(Category parent)
    {
        if (parent is null)
            throw new DomainValidationException(ErrorCodes.Domain.Category.ParentRequired);

        if (parent.Id == this.Id)
            throw new DomainValidationException(ErrorCodes.Category.ParentSelf);

        Parent = parent;
        ParentId = parent.Id;
    }

    private void ClearParent()
    {
        Parent = null;
        ParentId = null;
    }
}


