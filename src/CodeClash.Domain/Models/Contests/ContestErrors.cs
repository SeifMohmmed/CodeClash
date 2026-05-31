using CodeClash.Domain.Abstractions;

namespace CodeClash.Domain.Models.Contests;
public static class ContestErrors
{
    public static readonly Error NotFound = new(
    "Contest.NotFound",
    "The contest with the specified identifier was not found");

    public static readonly Error NotStarted = new(
    "Contest.NotStarted",
    "The contest with the specified identifier was not started yet!");

    public static readonly Error Ended = new(
    "Contest.Ended",
    "The contest with the specified identifier Ended!");

}
