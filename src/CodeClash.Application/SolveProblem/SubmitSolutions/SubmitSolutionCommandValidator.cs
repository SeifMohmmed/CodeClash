using CodeClash.Application.Abstractions.File;
using FluentValidation;

namespace CodeClash.Application.SolveProblem.SubmitSolutions;
internal sealed class SubmitSolutionCommandValidator
    : AbstractValidator<SubmitSolutionCommand>
{
    public SubmitSolutionCommandValidator(IFileService fileService)
    {

        RuleFor(x => x)
       .NotEmpty().WithMessage("Input cannot be empty.")
       .NotNull().WithMessage("Input cannot be null.")
       .Must((model) => fileService.CheckFileExtension(model.Code, model.Language))
       .WithMessage("File extension doesn't match the requested language!");

        RuleFor(x => x)
            .NotEmpty().WithMessage("Input cannot be empty.")
            .NotNull().WithMessage("Input cannot be null.")
            .Must((model) => fileService.CheckFileExtension(model.Code, model.Language))
            .WithMessage("Unsupported language!");

        RuleFor(x => x.ProblemId).NotEmpty().NotNull();
        RuleFor(x => x.ContestId).NotEmpty().NotNull();
    }
}
