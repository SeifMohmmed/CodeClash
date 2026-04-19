using CodeClash.Domain.Abstractions;

namespace CodeClash.Domain.Models.Topics;
public static class TopicErrors
{
    public static Error NotFound(Guid topicId) =>
        new("Topic.NotFound", $"Topic with id '{topicId}' was not found.");
}
