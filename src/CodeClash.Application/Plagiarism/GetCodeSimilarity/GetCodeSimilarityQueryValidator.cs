using FluentValidation;

namespace CodeClash.Application.Plagiarism.GetCodeSimilarity;

public sealed class GetCodeSimilarityQueryValidator : AbstractValidator<GetCodeSimilarityQuery>
{
    public GetCodeSimilarityQueryValidator()
    {
        RuleFor(x => x.Code1)
            .NotEmpty().WithMessage("Code1 is required.")
            .MaximumLength(10000).WithMessage("Code1 must not exceed 10000 characters.");

        RuleFor(x => x.Code2)
            .NotEmpty().WithMessage("Code2 is required.")
            .MaximumLength(10000).WithMessage("Code2 must not exceed 10000 characters.");
    }
}
