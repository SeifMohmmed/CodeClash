using CodeClash.Domain.Models.Blogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeClash.Infrastructure.Configurations;
internal sealed class BlogImageConfiguration
    : IEntityTypeConfiguration<BlogImage>
{
    public void Configure(EntityTypeBuilder<BlogImage> builder)
    {
        builder.HasOne(ti => ti.Blog)
            .WithMany(t => t.Images)
            .HasForeignKey(ti => ti.BlogId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
