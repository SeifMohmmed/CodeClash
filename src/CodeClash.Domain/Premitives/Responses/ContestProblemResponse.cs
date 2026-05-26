namespace CodeClash.Domain.Premitives.Responses;
public sealed class ContestProblemResponse
{
    public Guid ContestId { get; set; }
    public string Name { get; set; }
    public Difficulty Difficulty { get; set; }

    public decimal RunTimeLimit { get; set; }
    public MemoryLimit MemoryLimit { get; set; }

    public string Description { get; set; }
    public ContestPoints ContestPoints { get; set; }
}
