using System.ComponentModel.DataAnnotations.Schema;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;

namespace CodeClash.Domain.Models.Submits;
public class Submit : BaseEntity
{
    public string UserId { get; set; }

    public Guid ProblemId { get; set; }

    public Guid? ContestId { get; set; }

    public string? Error { get; set; }

    public decimal? SubmitTime { get; set; } // Execution Time

    public decimal? SubmitMemory { get; set; } // Execution Memory

    public SubmissionResult Result { get; set; }

    public string Code { get; set; }

    public DateTime SubmissionDate { get; set; } = DateTime.Now;

    public Language Language { get; set; }


    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; }

    [ForeignKey(nameof(ProblemId))]
    public Problem Problem { get; set; }

    [ForeignKey(nameof(ContestId))]
    public Contest Contest { get; set; }
}
