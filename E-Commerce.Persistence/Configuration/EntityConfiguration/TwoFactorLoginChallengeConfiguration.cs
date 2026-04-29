using E_Commerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class TwoFactorLoginChallengeConfiguration : IEntityTypeConfiguration<TwoFactorLoginChallenge>
{
    public void Configure(EntityTypeBuilder<TwoFactorLoginChallenge> builder)
    {
        builder.ToTable("TwoFactorLoginChallenges");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ChallengeId)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(x => x.ChallengeId)
               .IsUnique();

        builder.Property(x => x.AttemptCount)
               .IsRequired();

        builder.Property(x => x.MaxAttempts)
               .IsRequired();

        builder.Property(x => x.ExpiresAt)
               .IsRequired();

        builder.Property(x => x.VerifiedAt);

        builder.HasIndex(x => x.UserId);

        builder.HasOne<User>()
               .WithMany() // NOT inside aggregate
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}