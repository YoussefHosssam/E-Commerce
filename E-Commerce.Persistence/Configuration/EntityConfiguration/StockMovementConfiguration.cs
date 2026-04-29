using E_Commerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovements");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(30);

        builder.Property(x => x.QuantityDelta).IsRequired();
        builder.Property(x => x.Reason).HasMaxLength(300);

        builder.HasOne(x => x.Variant)
               .WithMany()
               .HasForeignKey(x => x.VariantId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ActorUser)
               .WithMany()
               .HasForeignKey(x => x.ActorUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.VariantId);
        builder.HasIndex(x => x.ActorUserId);
    }
}