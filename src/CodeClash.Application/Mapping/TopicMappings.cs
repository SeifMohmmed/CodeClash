using CodeClash.Application.DTO;
using CodeClash.Application.Topics.CreateTopics;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Models.Topics;

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

    public static CreateTopicResponse ToCreateResponse(Topic topic)
    {
        return new CreateTopicResponse
        {
            Id = topic.Id,
            Name = topic.Name
        };
    }
}
