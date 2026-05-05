using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Topics.CreateTopics;
public sealed record CreateTopicCommand(
    string Name) : ICommand<CreateTopicResponse>;
