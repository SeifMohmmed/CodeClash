using System.ComponentModel.DataAnnotations.Schema;
using CodeClash.Domain.Models.Comments;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Premitives;

namespace CodeClash.Domain.Models.Blogs;
public sealed class Blog : Entity
{
    public string CreatorId { get; set; }

    public string Content { get; set; }

    public string Title { get; set; }

    public Contest Contest { get; set; }


    [ForeignKey(nameof(CreatorId))]
    public User BlogCreator { get; set; }
    public ICollection<BlogImage> Images { get; set; }
    public ICollection<Comment> Comments { get; set; }
}
