using CodeClash.Domain.Abstractions;

namespace CodeClash.Domain.Models.Contests;
public static class ContestErrors
{
    public static readonly Error NotFound = new(
    "Contest.NotFound",
    "The contest with the specified identifier was not found");

}
