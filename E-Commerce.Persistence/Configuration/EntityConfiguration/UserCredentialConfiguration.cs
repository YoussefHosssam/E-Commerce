// Infrastructure/Persistence/Configurations/UserCredentialConfiguration.cs

using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserCredentialConfiguration : IEntityTypeConfiguration<UserCredential>
{
    public void Configure(EntityTypeBuilder<UserCredential> builder)
    {
        builder.ToTable("UserCredentials");

        builder.HasKey(x => x.UserId);

        builder.Property(x => x.PasswordHash)
               .IsRequired()
               .HasConversion(ValueConverters.StructString<PasswordHash>())
               .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.UpdatedAt)
               .IsRequired();

        builder.HasOne<User>()
               .WithOne(x => x.Credential)
               .HasForeignKey<UserCredential>(x => x.UserId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}