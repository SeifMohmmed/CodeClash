using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Premitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeClash.Infrastructure.Configurations;
internal sealed class UserConfiguration
    : IEntityTypeConfiguration<User>
{
    public void Configure(
        EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
               .HasMaxLength(500);

        builder.Property(u => u.Email)
               .HasMaxLength(300);

        builder.Property(u => u.IdentityId)
               .HasMaxLength(500);

        builder.Property(u => u.Name)
               .HasMaxLength(100);

        builder.HasIndex(u => u.Email)
               .IsUnique();

        builder.HasIndex(u => u.IdentityId)
               .IsUnique();

        builder.Property(u => u.RankName)
               .HasConversion<string>()
               .HasDefaultValue(UserStatus.UnRanked);

        builder.Property(u => u.Rating)
               .HasDefaultValue((short)0);
    }
}
