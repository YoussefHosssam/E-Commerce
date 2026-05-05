using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Exceptions;
using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Domain.Entities;

public sealed class Order : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!; // EF navigation

    public string OrderNumber { get; private set; } = default!; // unique (DB unique index)
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public decimal Subtotal { get; private set; }
    public decimal ShippingFee { get; private set; }
    public decimal DiscountTotal { get; private set; }
    public decimal TaxTotal { get; private set; }
    public decimal GrandTotal { get; private set; }

    public CurrencyCode Currency { get; private set; } = CurrencyCode.Create("EGP");

    public JsonText ShippingAddressJson { get; private set; } = JsonText.Create("{}");
    public JsonText BillingAddressJson { get; private set; } = JsonText.Create("{}");

    public string? Notes { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private readonly List<Payment> _payments = new();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    private Order() { } // EF

    private Order(
        Guid userId,
        string orderNumber,
        CurrencyCode currency,
        JsonText shippingAddress,
        JsonText billingAddress,
        string? notes,
        DateTimeOffset now)
    {
        UserId = userId;
        OrderNumber = orderNumber;
        Currency = currency;

        ShippingAddressJson = shippingAddress;
        BillingAddressJson = billingAddress;

        Notes = notes;

        Status = OrderStatus.Pending;
        Touch(now);
    }

    public static Order Create(
        Guid userId,
        string orderNumber,
        CurrencyCode currency,
        JsonText shippingAddress,
        JsonText? billingAddress,
        string? notes,
        DateTimeOffset now)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException(OrderErrors.UserIdRequired);

        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new DomainValidationException(OrderErrors.NumberRequired);

        orderNumber = orderNumber.Trim();

        if (orderNumber.Length > 40)
            throw new DomainValidationException(OrderErrors.NumberTooLong);

        if (currency.Equals(default(CurrencyCode)))
            throw new DomainValidationException(OrderErrors.CurrencyRequired);

        if (shippingAddress.Equals(default(JsonText)))
            throw new DomainValidationException(OrderErrors.ShippingAddress.Required);

        billingAddress ??= shippingAddress;

        if (now == default)
            throw new DomainValidationException(OrderErrors.NowRequired);

        if (notes is not null)
        {
            notes = notes.Trim();
            if (notes.Length == 0) notes = null;
            if (notes is not null && notes.Length > 500)
                throw new DomainValidationException(OrderErrors.NotesTooLong);
        }

        return new Order(userId, orderNumber, currency, shippingAddress, billingAddress.Value, notes, now);
    }

    // ---------------- Domain behaviors ----------------

    public void AddItem(OrderItem item, DateTimeOffset now)
    {
        EnsureEditable();

        if (item is null)
            throw new DomainValidationException(OrderErrors.ItemRequired);

        // rule: currency must match
        if (!item.Currency.Equals(Currency))
            throw new DomainValidationException(OrderErrors.ItemCurrencyMismatch);

        _items.Add(item);
        RecalculateTotals();
        Touch(now);
    }

    public void RemoveItem(Guid orderItemId, DateTimeOffset now)
    {
        EnsureEditable();

        var idx = _items.FindIndex(i => i.Id == orderItemId);
        if (idx < 0)
            throw new DomainValidationException(OrderErrors.ItemNotFound);

        _items.RemoveAt(idx);
        RecalculateTotals();
        Touch(now);
    }

    public void SetShippingFee(decimal fee, DateTimeOffset now)
    {
        EnsureEditable();

        if (fee < 0)
            throw new DomainValidationException(OrderErrors.ShippingFeeInvalid);

        ShippingFee = fee;
        RecalculateTotals();
        Touch(now);
    }

    public void ApplyDiscount(decimal discountTotal, DateTimeOffset now)
    {
        EnsureEditable();

        if (discountTotal < 0)
            throw new DomainValidationException(OrderErrors.DiscountInvalid);

        DiscountTotal = discountTotal;
        RecalculateTotals();
        Touch(now);
    }

    public void SetTaxTotal(decimal taxTotal, DateTimeOffset now)
    {
        EnsureEditable();

        if (taxTotal < 0)
            throw new DomainValidationException(OrderErrors.TaxInvalid);

        TaxTotal = taxTotal;
        RecalculateTotals();
        Touch(now);
    }

    public void UpdateAddresses(JsonText shipping, JsonText billing, DateTimeOffset now)
    {
        EnsureEditable();

        ShippingAddressJson = shipping.Equals(default(JsonText)) ? throw new DomainValidationException(OrderErrors.ShippingAddress.Required) : shipping;
        BillingAddressJson = billing.Equals(default(JsonText)) ? throw new DomainValidationException(OrderErrors.BillingAddress.Required) : billing;

        Touch(now);
    }

    public void AddPayment(Payment payment, DateTimeOffset now)
    {
        if (payment is null)
            throw new DomainValidationException(OrderErrors.PaymentRequired);

        if (payment.OrderId != this.Id)
            throw new DomainValidationException(OrderErrors.PaymentMismatch);

        // Optional rule: prevent adding payments after cancellation/refund etc. (??? enums ????)
        _payments.Add(payment);
        Touch(now);
    }

    public void MarkPaid(DateTimeOffset now)
    {
        // Usually paid after captured payment(s) cover GrandTotal (Application layer checks)
        if (Status != OrderStatus.Pending)
            throw new DomainValidationException(OrderErrors.StatusInvalidTransition);

        Status = OrderStatus.Paid;
        Touch(now);
    }

    public void Cancel(string? reason, DateTimeOffset now)
    {
        if (Status is OrderStatus.Shipped or OrderStatus.Delivered)
            throw new DomainValidationException(OrderErrors.CancelNotAllowed);

        Status = OrderStatus.Cancelled;

        if (reason is not null)
        {
            reason = reason.Trim();
            if (reason.Length > 300)
                throw new DomainValidationException(OrderErrors.CancelReasonTooLong);
            Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason;
        }

        Touch(now);
    }

    private void EnsureEditable()
    {
        if (Status is not OrderStatus.Pending)
            throw new DomainValidationException(OrderErrors.NotEditable);
    }

    private void RecalculateTotals()
    {
        Subtotal = _items.Sum(i => i.LineTotal);

        // grand = subtotal + ship + tax - discount
        var total = Subtotal + ShippingFee + TaxTotal - DiscountTotal;

        if (total < 0)
            throw new DomainValidationException(OrderErrors.TotalInvalid);

        GrandTotal = total;
    }

    private void Touch(DateTimeOffset now)
    {
        if (now == default)
            throw new DomainValidationException(OrderErrors.NowRequired);

        UpdatedAt = now;
    }
}

