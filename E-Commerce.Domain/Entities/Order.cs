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
            throw new DomainValidationException(ErrorCodes.Domain.Order.UserIdRequired);

        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new DomainValidationException(ErrorCodes.Domain.Order.NumberRequired);

        orderNumber = orderNumber.Trim();

        if (orderNumber.Length > 40)
            throw new DomainValidationException(ErrorCodes.Domain.Order.NumberTooLong);

        if (currency.Equals(default(CurrencyCode)))
            throw new DomainValidationException(ErrorCodes.Domain.Order.CurrencyRequired);

        if (shippingAddress.Equals(default(JsonText)))
            throw new DomainValidationException(ErrorCodes.Domain.Order.ShippingAddressRequired);

        billingAddress ??= shippingAddress;

        if (now == default)
            throw new DomainValidationException(ErrorCodes.Domain.Order.NowRequired);

        if (notes is not null)
        {
            notes = notes.Trim();
            if (notes.Length == 0) notes = null;
            if (notes is not null && notes.Length > 500)
                throw new DomainValidationException(ErrorCodes.Domain.Order.NotesTooLong);
        }

        return new Order(userId, orderNumber, currency, shippingAddress, billingAddress.Value, notes, now);
    }

    // ---------------- Domain behaviors ----------------

    public void AddItem(OrderItem item, DateTimeOffset now)
    {
        EnsureEditable();

        if (item is null)
            throw new DomainValidationException(ErrorCodes.Domain.Order.ItemRequired);

        // rule: currency must match
        if (!item.Currency.Equals(Currency))
            throw new DomainValidationException(ErrorCodes.Domain.Order.ItemCurrencyMismatch);

        _items.Add(item);
        RecalculateTotals();
        Touch(now);
    }

    public void RemoveItem(Guid orderItemId, DateTimeOffset now)
    {
        EnsureEditable();

        var idx = _items.FindIndex(i => i.Id == orderItemId);
        if (idx < 0)
            throw new DomainValidationException(ErrorCodes.Domain.Order.ItemNotFound);

        _items.RemoveAt(idx);
        RecalculateTotals();
        Touch(now);
    }

    public void SetShippingFee(decimal fee, DateTimeOffset now)
    {
        EnsureEditable();

        if (fee < 0)
            throw new DomainValidationException(ErrorCodes.Domain.Order.ShippingFeeInvalid);

        ShippingFee = fee;
        RecalculateTotals();
        Touch(now);
    }

    public void ApplyDiscount(decimal discountTotal, DateTimeOffset now)
    {
        EnsureEditable();

        if (discountTotal < 0)
            throw new DomainValidationException(ErrorCodes.Domain.Order.DiscountInvalid);

        DiscountTotal = discountTotal;
        RecalculateTotals();
        Touch(now);
    }

    public void SetTaxTotal(decimal taxTotal, DateTimeOffset now)
    {
        EnsureEditable();

        if (taxTotal < 0)
            throw new DomainValidationException(ErrorCodes.Domain.Order.TaxInvalid);

        TaxTotal = taxTotal;
        RecalculateTotals();
        Touch(now);
    }

    public void UpdateAddresses(JsonText shipping, JsonText billing, DateTimeOffset now)
    {
        EnsureEditable();

        ShippingAddressJson = shipping.Equals(default(JsonText)) ? throw new DomainValidationException(ErrorCodes.Domain.Order.ShippingAddressRequired) : shipping;
        BillingAddressJson = billing.Equals(default(JsonText)) ? throw new DomainValidationException(ErrorCodes.Domain.Order.BillingAddressRequired) : billing;

        Touch(now);
    }

    public void AddPayment(Payment payment, DateTimeOffset now)
    {
        if (payment is null)
            throw new DomainValidationException(ErrorCodes.Domain.Order.PaymentRequired);

        if (payment.OrderId != this.Id)
            throw new DomainValidationException(ErrorCodes.Domain.Order.PaymentMismatch);

        // Optional rule: prevent adding payments after cancellation/refund etc. (??? enums ????)
        _payments.Add(payment);
        Touch(now);
    }

    public void MarkPaid(DateTimeOffset now)
    {
        // Usually paid after captured payment(s) cover GrandTotal (Application layer checks)
        if (Status != OrderStatus.Pending)
            throw new DomainValidationException(ErrorCodes.Domain.Order.StatusInvalidTransition);

        Status = OrderStatus.Paid;
        Touch(now);
    }

    public void Cancel(string? reason, DateTimeOffset now)
    {
        // cancel rules ??? ????? (????? ??? ????? ???)
        if (Status is OrderStatus.Shipped or OrderStatus.Delivered)
            throw new DomainValidationException(ErrorCodes.Domain.Order.CancelNotAllowed);

        Status = OrderStatus.Cancelled;

        if (reason is not null)
        {
            reason = reason.Trim();
            if (reason.Length > 300)
                throw new DomainValidationException(ErrorCodes.Domain.Order.CancelReasonTooLong);
            Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason;
        }

        Touch(now);
    }

    private void EnsureEditable()
    {
        // ????? ??????? ??? enum ?????
        if (Status is not OrderStatus.Pending)
            throw new DomainValidationException(ErrorCodes.Domain.Order.NotEditable);
    }

    private void RecalculateTotals()
    {
        Subtotal = _items.Sum(i => i.LineTotal);

        // grand = subtotal + ship + tax - discount
        var total = Subtotal + ShippingFee + TaxTotal - DiscountTotal;

        if (total < 0)
            throw new DomainValidationException(ErrorCodes.Domain.Order.TotalInvalid);

        GrandTotal = total;
    }

    private void Touch(DateTimeOffset now)
    {
        if (now == default)
            throw new DomainValidationException(ErrorCodes.Domain.Order.NowRequired);

        UpdatedAt = now;
    }
}

