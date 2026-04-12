using CodeClash.Application.DTO;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Problems.GetProblemById;
public sealed class GetProblemByIdResponse
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; }
    public List<TestCasesDto> TasteCases { get; set; }
    public List<TopicDto> Topics { get; set; }

    public decimal Accepted { get; set; }
    public decimal Submissions { get; set; }
    public decimal AcceptanceRate => Submissions > 0 ? Accepted / Submissions * 100 : 0;

    public bool IsSolved { get; set; }
}
