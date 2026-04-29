using E_Commerce.Domain.ValueObjects;
using E_Commerce.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrastructure.Persistence.Configurations.OwnedTypes;

internal static class MoneyOwnedMapping
{
    public static OwnedNavigationBuilder<TEntity, Money> MapMoney<TEntity>(
        this OwnedNavigationBuilder<TEntity, Money> money,
        string amountColumn,
        string currencyColumn)
        where TEntity : class
    {
        money.Property(m => m.Amount)
             .HasPrecision(18, 2)
             .HasColumnName(amountColumn);

        money.Property(m => m.Currency)
             .HasConversion(ValueConverters.StructString<CurrencyCode>())
             .HasMaxLength(3)
             .HasColumnName(currencyColumn);

        return money;
    }
}