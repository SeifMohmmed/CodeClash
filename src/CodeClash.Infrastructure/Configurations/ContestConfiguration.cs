using CodeClash.Domain.Models.Contests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeClash.Infrastructure.Configurations;
internal sealed class ContestConfiguration
    : IEntityTypeConfiguration<Contest>
{
    public void Configure(EntityTypeBuilder<Contest> builder)
    {
        builder.HasOne(c => c.ProblemSetter)
            .WithMany(u => u.Contests)
            .HasForeignKey(c => c.SetterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(c => c.Name)
            .IsRequired();
    }
}
