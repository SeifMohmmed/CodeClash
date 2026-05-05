using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Topics.GetAllTopics;
internal sealed class GetAllTopicsQueryHandler(
    ITopicRepository topicRepository) : IQueryHandler<GetAllTopicsQuery, List<GetAllTopicsResponse>>
{
    public async Task<Result<List<GetAllTopicsResponse>>> Handle(
        GetAllTopicsQuery request,
        CancellationToken cancellationToken)
    {
        var topics = await topicRepository.GetAllAsync();
        if (topics is null)
        {
            return Result.Failure<List<GetAllTopicsResponse>>(new Error("Topic.Not.Found", "Topics Not Found!"));
        }

        var response = topics.Select(t => new GetAllTopicsResponse
        {
            Id = t.Id,
            Name = t.Name
        }).ToList();

        return Result.Success(response);
    }
}
