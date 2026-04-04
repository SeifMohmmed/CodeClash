namespace CodeClash.Domain.Premitives.Responses;
public sealed class MemoryLimitExceedResponse : BaseSubmissionResponse
{
    public int TestCaseNumber { get; set; }
    public decimal ExecutionMemory { get; set; }
}
