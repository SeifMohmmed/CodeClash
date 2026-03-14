using CodeClash.Domain.Abstractions;

namespace CodeClash.Domain.Models.Problems;
public static class ProblemErrors
{
    public static readonly Error NotFound = new(
        "Problem.NotFound",
        "The problem with the specified identifier was not found");

    public static readonly Error AlreadyExists = new(
        "Problem.AlreadyExists",
        "A problem with the same title already exists");

    public static readonly Error InvalidDifficulty = new(
        "Problem.InvalidDifficulty",
        "The provided difficulty level is invalid");

    public static readonly Error InvalidInput = new(
        "Problem.InvalidInput",
        "The provided problem data is invalid");

    public static readonly Error CannotDelete = new(
        "Problem.CannotDelete",
        "The problem cannot be deleted because it is referenced by submissions");
}
