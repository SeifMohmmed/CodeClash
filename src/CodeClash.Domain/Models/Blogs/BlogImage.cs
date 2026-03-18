using CodeClash.Domain.Premitives;

namespace CodeClash.Domain.Models.Blogs;
public sealed class BlogImage : Entity
{
    public Guid BlogId { get; set; }

    public string ImagePath { get; set; }

    public Blog Blog { get; set; }
}
