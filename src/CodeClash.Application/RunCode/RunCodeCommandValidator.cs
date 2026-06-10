using System.Text.Json;
using CodeClash.Application.DTO;
using FluentValidation;

namespace CodeClash.Application.RunCode;

internal sealed class RunCodeCommandValidator
    : AbstractValidator<RunCodeCommand>
{
    public RunCodeCommandValidator()
    {
        RuleFor(x => x.Language)
    .IsInEnum()
    .WithMessage("Invalid language value. Must be a defined enum.");

        // Validate Code

        RuleFor(x => x.Language)
            .IsInEnum()
            .WithMessage("Invalid language value. Must be a defined enum.");

        // Validate ProblemId
        RuleFor(x => x.ProblemId)
            .NotEmpty()
            .WithMessage("ProblemId must be greater than 0.");

        // Validate CustomTestcasesJson
        RuleFor(x => x.CustomTestcasesJson)
            .NotEmpty()
            .WithMessage("CustomTestcasesJson is required.")
            .Must(BeValidJsonArray)
            .WithMessage("CustomTestcasesJson must be a valid JSON array.");
    }

    private bool BeValidJsonArray(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        try
        {
            var testcases = JsonSerializer.Deserialize<List<CustomTestcaseDto>>(json);
            return testcases != null;
        }

        catch
        {
            return false;
        }
    }
}
