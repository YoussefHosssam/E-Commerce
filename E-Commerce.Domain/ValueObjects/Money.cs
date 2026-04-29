using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Domain.ValueObjects;

public sealed class Money 
{
    public decimal Amount { get; }
    public CurrencyCode Currency { get; }

    private Money() { } // EF

    private Money(decimal amount, CurrencyCode currency)
    {
        if (amount < 0)
            throw new DomainValidationException(ErrorCodes.ValueObjects.MoneyAmountInvalid);

        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, CurrencyCode currency)
        => new(amount, currency);
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainValidationException(ErrorCodes.ValueObjects.CurrencyInvalid);

        return new Money(Amount + other.Amount, Currency);
    }
}
