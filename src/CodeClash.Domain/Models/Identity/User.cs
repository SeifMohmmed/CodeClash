using CodeClash.Domain.Models.Blogs;
using CodeClash.Domain.Models.Comments;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Models.Submits;
using CodeClash.Domain.Premitives;

namespace CodeClash.Domain.Models.Identity;
public class User
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    /// <summary>
    /// We'll use this to store the IdentityId from the Identity Provider
    /// This could be any identity provider Like Azure AD, Okta, Autho, etc.
    /// </summary>
    public string IdentityId { get; set; }

    public UserStatus RankName { get; set; } = UserStatus.UnRanked; // ex: pupil

    public string? ImagePath { get; set; }

    public short Rating { get; set; }

    public Gender? Gender { get; set; }

    public ICollection<Contest> Contests { get; set; } // Contests created by this user (Setter)
    public ICollection<Problem> Problems { get; set; } // Problems set by this user (Setter)
    public ICollection<Blog> Blogs { get; set; } // Tutorials created by this user (Setter)
    public ICollection<UserContest> Registrations { get; set; } // Contests registered by this user
    public ICollection<Submit> Submissions { get; set; }
    public ICollection<Comment> Comments { get; set; }
}
