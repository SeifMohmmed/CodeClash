using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Topics.GetAllTopics;

public sealed record GetAllTopicsQuery : IQuery<List<GetAllTopicsResponse>>;
