namespace CodeClash.Application.Problems.GetAllProblem;
public sealed class GetAllProblemResponse
{
    public string Name { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public List<Guid> Topics { get; set; } = new List<Guid>();
    public bool IsSolved { get; set; }
}
