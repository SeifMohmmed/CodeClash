using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Topics;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Topics.CreateTopics;
internal sealed class CreateTopicCommandHandler(
    ITopicRepository topicRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateTopicCommand, CreateTopicResponse>
{
    public async Task<Result<CreateTopicResponse>> Handle(
        CreateTopicCommand request,
        CancellationToken cancellationToken)
    {
        var topic = new Topic
        {
            Name = request.Name,
        };

        topicRepository.Add(topic);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = TopicMappings.ToCreateResponse(topic);

        return Result.Success(response);
    }
}
