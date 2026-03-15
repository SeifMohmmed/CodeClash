using CodeClash.Domain.Premitives;

namespace CodeClash.Domain.Models.Problems;
public sealed class ProblemImage : BaseEntity
{
    public Guid ProblemId { get; set; }

    public string ImagePath { get; set; }

    public Problem Problem { get; set; }
}
