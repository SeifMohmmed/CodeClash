namespace CodeClash.Domain.Premitives;

public sealed record ProblemStats(
    int AcceptedCount,
    int TotalCount)
{
    public double AcceptanceRate =>
        TotalCount == 0 ? 0 : (double)AcceptedCount / TotalCount;
}
