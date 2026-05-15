using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Exceptions;
using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Domain.Entities;

public sealed class Category : BaseEntity
{
    public Guid? ParentId { get; private set; }
    public string Name { get; private set; }
    public Category? Parent { get; private set; } // EF navigation

    private readonly List<Category> _children = new();
    public IReadOnlyCollection<Category> Children => _children.AsReadOnly();

    public Slug Slug { get; private set; } = default!; // unique (DB unique index)
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset? UpdatedAt { get; private set; } = null;
    // Usually not part of category aggregate (query navigation)
    private readonly List<Product> _products = new();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Category() { } // EF

    private Category(Guid? parentId , string name, Slug slug, int sortOrder)
    {
        ParentId = parentId;
        Name = name;
        Slug = slug;
        SortOrder = sortOrder;
        IsActive = true;
    }

    public static Category Create(Guid? parentId , string name, Slug slug, int sortOrder = 0)
    {
        if (parentId.HasValue && parentId.Value == Guid.Empty)
            throw new DomainValidationException(CategoryErrors.ParentInvalid);
        if (string.IsNullOrEmpty(name) || name.Length < 5 || name.Length > 50)
            throw new DomainValidationException(CategoryErrors.InvalidName);
        if (slug.Equals(default(Slug)))
            throw new DomainValidationException(CategoryErrors.SlugRequired);
        if (sortOrder < 0)
            throw new DomainValidationException(CategoryErrors.SortOrderInvalid);

        return new Category(parentId,name, slug, sortOrder);
    }

    // --------- Behaviors ---------

    public void ChangeSlug(Slug slug , DateTimeOffset now)
    {
        if (slug.Equals(default(Slug)))
            throw new DomainValidationException(CategoryErrors.SlugRequired);

        // uniqueness globally is DB/application responsibility
        Slug = slug;
        Touch(now);
    }

    public void SetSortOrder(int sortOrder , DateTimeOffset now)
    {
        if (sortOrder < 0)
            throw new DomainValidationException(CategoryErrors.SortOrderInvalid);

        SortOrder = sortOrder;
        Touch(now);
    }
    public void ChangeName(string name , DateTimeOffset now)
    {
        if (string.IsNullOrEmpty(name) || name.Length < 5 || name.Length > 50)
            throw new DomainValidationException(CategoryErrors.InvalidName);
        Name = name;
        Touch(now);
    }
    public void ChangeParent(Category? parent , DateTimeOffset now)
    {
        if (parent is null)
        {
            ClearParent();
            return;
        }

        SetParent(parent);
        Touch(now);
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public void AddChild(Category child , DateTimeOffset now)
    {
        if (child is null)
            throw new DomainValidationException(CategoryErrors.ChildRequired);

        if (child.Id == this.Id)
            throw new DomainValidationException(CategoryErrors.ChildSelf);

        if (_children.Any(c => c.Id == child.Id))
            throw new DomainValidationException(CategoryErrors.ChildDuplicate);

        child.SetParent(this);

        _children.Add(child);
        Touch(now);
    }

    public void RemoveChild(Guid childId , DateTimeOffset now)
    {
        var idx = _children.FindIndex(c => c.Id == childId);
        if (idx < 0)
            throw new DomainValidationException(CategoryErrors.ChildNotFound);

        _children[idx].ClearParent();
        _children.RemoveAt(idx);
        Touch(now);

    }

    private void SetParent(Category parent)
    {
        if (parent is null)
            throw new DomainValidationException(CategoryErrors.ParentRequired);

        if (parent.Id == this.Id)
            throw new DomainValidationException(CategoryErrors.ParentSelf);

        Parent = parent;
        ParentId = parent.Id;
    }

    private void ClearParent()
    {
        Parent = null;
        ParentId = null;
    }

    private void Touch(DateTimeOffset now)
    {
        UpdatedAt = now;
    }
}


