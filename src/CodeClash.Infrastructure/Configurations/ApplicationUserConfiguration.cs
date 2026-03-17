using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Premitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeClash.Infrastructure.Configurations;
internal sealed class ApplicationUserConfiguration
    : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(
        EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("users");

        builder.Property(u => u.RankName)
                      .HasConversion(uStatus => uStatus.ToString(), OStatus => Enum.Parse<UserStatus>(OStatus));

        builder.Property(u => u.Email)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique();
    }
}
