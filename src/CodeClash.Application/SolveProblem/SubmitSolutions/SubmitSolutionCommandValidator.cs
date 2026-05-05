using FluentValidation;

namespace CodeClash.Application.SolveProblem.SubmitSolutions;
internal sealed class SubmitSolutionCommandValidator
    : AbstractValidator<SubmitSolutionCommand>
{
    public SubmitSolutionCommandValidator()
    {

        // RuleFor(x => x)
        //.NotEmpty().WithMessage("Input cannot be empty.")
        //.NotNull().WithMessage("Input cannot be null.")
        //.Must((model) => fileService.CheckFileExtension(model.Code, model.Language))
        //.WithMessage("File extension doesn't match the requested language!");

        // RuleFor(x => x)
        //     .NotEmpty().WithMessage("Input cannot be empty.")
        //     .NotNull().WithMessage("Input cannot be null.")
        //     .Must((model) => fileService.CheckFileExtension(model.Code, model.Language))
        //     .WithMessage("Unsupported language!");

        // RuleFor(x => x.ProblemId).NotEmpty().NotNull();
        // RuleFor(x => x.ContestId).NotEmpty().NotNull();


        RuleFor(x => x.Language)
            .IsInEnum()
            .WithMessage("Invalid language value. Must be a defined enum.");

        // Validate Code
        RuleFor(x => x.Code)
            .NotNull()
            .WithMessage("Code file is required.")
            .Must(file => file.Length > 0)
            .WithMessage("Code file cannot be empty.");

        // Validate ProblemId
        RuleFor(x => x.ProblemId)
            .NotEmpty().NotNull();
    }
}
