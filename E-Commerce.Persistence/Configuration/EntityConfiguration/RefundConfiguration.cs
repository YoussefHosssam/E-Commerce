using E_Commerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class RefundConfiguration : IEntityTypeConfiguration<Refund>
{
    public void Configure(EntityTypeBuilder<Refund> builder)
    {
        builder.ToTable("Refunds");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PaymentId).IsRequired();

        builder.Property(x => x.ProviderRefundId).HasMaxLength(120);

        builder.Property(x => x.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(30);

        builder.Property(x => x.Amount).HasPrecision(18, 2);

        builder.Property(x => x.Reason).HasMaxLength(300);
        builder.Property(x => x.UpdatedAt);

        builder.HasIndex(x => x.PaymentId);
    }
}