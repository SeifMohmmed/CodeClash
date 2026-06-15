using FluentValidation;

namespace CodeClash.Application.EditUserDetails;

public sealed class EditUserDetailsCommandValidator
    : AbstractValidator<EditUserDetailsCommand>
{
    private static readonly string[] AllowedImageExtensions =
    [".jpg", ".jpeg", ".png", ".webp"];

    public EditUserDetailsCommandValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.")
            .When(x => x.Name is not null);

        RuleFor(x => x.Image)
            .Must(file => file is null || file.Length <= 5 * 1024 * 1024)
            .WithMessage("Image must not exceed 5MB.")
            .Must(file => file is null || AllowedImageExtensions
                .Contains(Path.GetExtension(file.FileName).ToLowerInvariant()))
            .WithMessage("Image must be a jpg, jpeg, png, or webp file.")
            .When(x => x.Image is not null);

        RuleFor(x => x)
            .Must(x => x.Name is not null || x.Image is not null || x.Gender is not null)
            .WithMessage("At least one field must be provided for update.");
    }
}
