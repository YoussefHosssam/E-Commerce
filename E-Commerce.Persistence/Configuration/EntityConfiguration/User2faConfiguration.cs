using E_Commerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class User2FaConfiguration : IEntityTypeConfiguration<UserTwoFactor>
{
    public void Configure(EntityTypeBuilder<UserTwoFactor> builder)
    {
        builder.ToTable("User2Fa");

        builder.HasKey(x => new { x.Id, x.UserId });

        builder.Property(x => x.IsEnabled).IsRequired();
        builder.Property(x => x.TotpSecretEncrypted).HasMaxLength(500);
        builder.Property(x => x.EnabledAt);
        builder.Property(x => x.DisabledAt);

        builder.HasOne(x => x.User)
               .WithOne(u => u.TwoFactorAuth)
               .HasForeignKey<UserTwoFactor>(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}