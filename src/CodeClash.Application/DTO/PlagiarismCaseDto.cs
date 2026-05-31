namespace CodeClash.Application.DTO;

public sealed class PlagiarismCaseDto
{
    public SubmitDto FirstSubmission { get; set; }
    public SubmitDto SecondSubmission { get; set; }
    public decimal Similarity { get; set; }
    public Guid ProblemId { get; set; }
}
