using CodeClash.Domain.Abstractions;
using FluentValidation;

namespace CodeClash.Application.Topics.CreateTopics;

public sealed class CreateTopicCommandValidator
    : AbstractValidator<CreateTopicCommand>
{
    public CreateTopicCommandValidator(ITopicRepository topicRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.")
            .MustAsync(async (name, cancellation) =>
                !await topicRepository.ExistsAsync(name, cancellation))
            .WithMessage("Name already exists.");
    }
}
