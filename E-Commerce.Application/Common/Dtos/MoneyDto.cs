using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Application.Common.Dtos;

public sealed record MoneyDto(decimal Amount, string Currency)
{
    public static MoneyDto FromMoney(Money money) => new(money.Amount, money.Currency.Value);
}
