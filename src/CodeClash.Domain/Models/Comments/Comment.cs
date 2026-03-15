using System.ComponentModel.DataAnnotations.Schema;
using CodeClash.Domain.Models.Blogs;
using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Premitives;

namespace CodeClash.Domain.Models.Comments;
public sealed class Comment : BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Path { get; set; }

    public string AuthorId { get; set; }

    public Guid BlogId { get; set; }

    public string Contest { get; set; }

    public bool IsEdited { get; set; }


    [ForeignKey(nameof(AuthorId))]
    public ApplicationUser Author { get; set; }

    [ForeignKey(nameof(BlogId))]
    public Blog Blog { get; set; }
}
