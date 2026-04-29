using E_Commerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class StockAlertConfiguration : IEntityTypeConfiguration<StockAlert>
{
    public void Configure(EntityTypeBuilder<StockAlert> builder)
    {
        builder.ToTable("StockAlerts");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(30);

        builder.Property(x => x.TriggeredAt);

        builder.HasOne(x => x.User)
               .WithMany(u => u.StockAlerts)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Variant)
               .WithMany()
               .HasForeignKey(x => x.VariantId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.VariantId }).IsUnique();
    }
}