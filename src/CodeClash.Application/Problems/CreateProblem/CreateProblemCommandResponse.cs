namespace CodeClash.Application.Problems.CreateProblem;
public sealed class CreateProblemCommandResponse
{
    public int ContestId { get; set; }
    public string Name { get; set; }
    public string Difficulty { get; set; }
    public decimal RunTimeLimit { get; set; }
    public decimal MemoryLimit { get; set; }
}
