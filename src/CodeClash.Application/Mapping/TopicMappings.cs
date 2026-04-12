using CodeClash.Application.DTO;
using CodeClash.Domain.Models.Problems;

namespace CodeClash.Application.Mapping;
internal static class TopicMappings
{
    public static TopicDto ToDto(ProblemTopic problemTopic)
    {
        return new TopicDto
        {
            Id = problemTopic.TopicId,
            Name = problemTopic.Topic?.Name ?? string.Empty
        };
    }
}
