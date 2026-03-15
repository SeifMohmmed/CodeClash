using System.ComponentModel.DataAnnotations.Schema;
using CodeClash.Domain.Models.Topics;

namespace CodeClash.Domain.Models.Problems;
public sealed class ProblemTopic
{
    public Guid ProblemId { get; set; }
    public Guid TopicId { get; set; }

    [ForeignKey(nameof(ProblemId))]
    public Problem Problem { get; set; }

    [ForeignKey(nameof(TopicId))]
    public Topic Topic { get; set; }
}
