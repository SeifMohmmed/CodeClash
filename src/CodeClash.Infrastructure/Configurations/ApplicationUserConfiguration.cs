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
        builder.Property(u => u.Status)
                      .HasConversion(uStatus => uStatus.ToString(), OStatus => Enum.Parse<Status>(OStatus));
    }
}
