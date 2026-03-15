using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;

namespace CodeClash.Domain.Models.Topics;
public sealed class Topic : BaseEntity
{
    public string Name { get; set; }

    public ICollection<ProblemTopic> ProblemTopics { get; set; }
}
