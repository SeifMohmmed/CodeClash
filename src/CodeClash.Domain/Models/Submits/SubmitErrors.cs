using CodeClash.Domain.Abstractions;

namespace CodeClash.Domain.Models.Submits;
public static class SubmitErrors
{
    public static readonly Error NotFound = new(
        "Submissions.NotFound",
        "The submissions with the specified identifier was not found");
}
