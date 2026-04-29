using E_Commerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class TwoFactorRecoveryCodeConfiguration : IEntityTypeConfiguration<TwoFactorRecoveryCode>
{
    public void Configure(EntityTypeBuilder<TwoFactorRecoveryCode> builder)
    {
        builder.ToTable("TwoFactorRecoveryCodes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CodeHash)
               .IsRequired()
               .HasMaxLength(512);

        builder.Property(x => x.UsedAt);

        builder.HasIndex(x => x.UserId);

        builder.HasOne<User>()
               .WithMany(x => x.RecoveryCodes)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}