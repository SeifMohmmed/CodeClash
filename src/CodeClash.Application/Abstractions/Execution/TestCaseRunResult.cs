using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Abstractions.Execution;
public class TestCaseRunResult
{
    public Guid TestCaseId { get; set; }

    public string Input { get; set; }
    public string ExpectedOutput { get; set; }
    public string? ActualOutput { get; set; }
    public SubmissionResult Result { get; set; } // passed or failed
    public decimal RunTime { get; set; }
    public decimal RunMemory { get; set; }
    public string Error { get; set; }
}
