using CodeClash.Domain.Models.Blogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeClash.Infrastructure.Configurations;
internal sealed class BlogConfiguration
    : IEntityTypeConfiguration<Blog>
{
    public void Configure(EntityTypeBuilder<Blog> builder)
    {
        builder.HasOne(b => b.BlogCreator)
                    .WithMany(u => u.Blogs)
                    .HasForeignKey(b => b.CreatorId)
                    .OnDelete(DeleteBehavior.Restrict);
    }
}
