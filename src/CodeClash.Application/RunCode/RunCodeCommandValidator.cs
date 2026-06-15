using System.Text.Json;
using CodeClash.Application.DTO;
using FluentValidation;

namespace CodeClash.Application.RunCode;

public sealed class RunCodeCommandValidator
    : AbstractValidator<RunCodeCommand>
{
    private const long MaxFileSizeInBytes = 1 * 1024 * 1024; // 1 MB
    public RunCodeCommandValidator()
    {
        // Validate Language
        RuleFor(x => x.Language)
            .IsInEnum()
            .WithMessage("Invalid language value. Must be a defined enum.");

        // Validate Code
        RuleFor(x => x.Code)
            .NotNull()
            .WithMessage("Code file is required.")
            .Must(file => file.Length > 0)
            .WithMessage("Code file must not be empty.")
            .Must(file => file.Length <= MaxFileSizeInBytes)
            .WithMessage("Code file must not exceed 1 MB.")
            .When(x => x.Code is not null);

        // Validate ProblemId
        RuleFor(x => x.ProblemId)
            .NotEmpty()
            .WithMessage("ProblemId is required.");

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
            return testcases is not null;
        }

        catch
        {
            return false;
        }
    }
}
