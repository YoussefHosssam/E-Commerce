using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Exceptions;

namespace E_Commerce.Domain.Entities;

public sealed class Cart : BaseEntity
{
    public Guid? UserId { get; private set; }           // nullable for anonymous
    public User? User { get; private set; }             // EF navigation
    public string? AnonymousToken { get; private set; } // unique cookie cartId

    public CartStatus Status { get; private set; } = CartStatus.Active;
    public DateTimeOffset? UpdatedAt { get; private set; }

    private readonly List<CartItem> _items = new();
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    private Cart() { } // EF

    private Cart(Guid? userId, string? anonymousToken, DateTimeOffset now)
    {
        UserId = userId;
        AnonymousToken = anonymousToken;
        Status = CartStatus.Active;
        Touch(now);
    }

    public static Cart CreateAnonymous(string anonymousToken, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(anonymousToken))
            throw new DomainValidationException(ErrorCodes.Domain.Cart.AnonymousTokenRequired);

        anonymousToken = anonymousToken.Trim();

        if (anonymousToken.Length > 128)
            throw new DomainValidationException(ErrorCodes.Domain.Cart.AnonymousTokenTooLong);

        if (now == default)
            throw new DomainValidationException(ErrorCodes.Domain.Cart.NowRequired);

        return new Cart(userId: null, anonymousToken: anonymousToken, now: now);
    }

    public static Cart CreateForUser(Guid userId, DateTimeOffset now)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException(ErrorCodes.Domain.Cart.UserIdRequired);

        if (now == default)
            throw new DomainValidationException(ErrorCodes.Domain.Cart.NowRequired);

        return new Cart(userId: userId, anonymousToken: null, now: now);
    }

    // ---------------- Behaviors ----------------

    public void AssignToUser(Guid userId, DateTimeOffset now)
    {
        EnsureActive();

        if (userId == Guid.Empty)
            throw new DomainValidationException(ErrorCodes.Domain.Cart.UserIdRequired);

        // ??? ?????? ????? ???? ?? anonymous
        UserId = userId;
        AnonymousToken = null;
        Touch(now);
    }

    public void SetStatus(CartStatus status, DateTimeOffset now)
    {
        if (!Enum.IsDefined(typeof(CartStatus), status))
            throw new DomainValidationException(ErrorCodes.Domain.Cart.StatusInvalid);

        Status = status;
        Touch(now);
    }

    public void AddItem(CartItem item, DateTimeOffset now)
    {
        EnsureActive();

        if (item is null)
            throw new DomainValidationException(ErrorCodes.Domain.Cart.ItemRequired);

        // rule: item must belong to this cart
        if (item.CartId != this.Id)
            throw new DomainValidationException(ErrorCodes.Domain.Cart.ItemMismatch);

        // rule: merge same variant into one line (???????)
        var existing = _items.FirstOrDefault(x => x.VariantId == item.VariantId);
        if (existing is null)
            _items.Add(item);
        else
            existing.IncreaseQuantity(item.Quantity, now);

        Touch(now);
    }

    public void RemoveItem(Guid cartItemId, DateTimeOffset now)
    {
        EnsureActive();

        var idx = _items.FindIndex(i => i.Id == cartItemId);
        if (idx < 0)
            throw new DomainValidationException(ErrorCodes.Domain.Cart.ItemNotFound);

        _items.RemoveAt(idx);
        Touch(now);
    }

    public void Clear(DateTimeOffset now)
    {
        EnsureActive();
        _items.Clear();
        Touch(now);
    }

    private void EnsureActive()
    {
        if (Status != CartStatus.Active)
            throw new DomainValidationException(ErrorCodes.Domain.Cart.NotActive);
    }

    private void Touch(DateTimeOffset now)
    {
        if (now == default)
            throw new DomainValidationException(ErrorCodes.Domain.Cart.NowRequired);

        UpdatedAt = now;
    }
}

