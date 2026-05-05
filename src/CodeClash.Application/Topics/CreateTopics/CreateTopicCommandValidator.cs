using CodeClash.Domain.Abstractions;
using FluentValidation;

namespace CodeClash.Application.Topics.CreateTopics;
public sealed class CreateTopicCommandValidator
    : AbstractValidator<CreateTopicCommand>
{
    private readonly ITopicRepository _topicRepository;
    public CreateTopicCommandValidator(ITopicRepository topicRepository)
    {
        _topicRepository = topicRepository;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MustAsync(async (name, cancellation) =>
                !await _topicRepository.ExistsAsync(name, cancellation))
            .WithMessage("Name already exists");
    }
}
