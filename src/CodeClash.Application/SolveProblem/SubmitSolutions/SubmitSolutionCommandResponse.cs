using CodeClash.Application.Abstractions.Execution;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.SolveProblem.SubmitSolutions;
public class SubmitSolutionCommandResponse
{
    public Guid ProblemId { get; set; }
    public List<TestCaseRunResult> TestCaseRuns { get; set; }
    public decimal SubmitTime { get; set; }
    public SubmissionResult SubmissionResult { get; set; }

    public string? Error { get; set; }
}
