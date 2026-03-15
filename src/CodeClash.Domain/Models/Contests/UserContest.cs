using System.ComponentModel.DataAnnotations.Schema;
using CodeClash.Domain.Models.Identity;

namespace CodeClash.Domain.Models.Contests;
public sealed class UserContest
{
    public string UserId { get; set; }

    public Guid ContestId { get; set; }

    public short RankChange { get; set; }   // the increase || decrease of rank


    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; }

    [ForeignKey(nameof(ContestId))]
    public Contest Contest { get; set; }

}
