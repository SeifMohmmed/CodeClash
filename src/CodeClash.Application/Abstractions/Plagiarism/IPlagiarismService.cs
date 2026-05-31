using CodeClash.Application.DTO;

namespace CodeClash.Application.Abstractions.Plagiarism;

public interface IPlagiarismService
{
    Task<IEnumerable<PlagiarismCaseDto>> GetPlagiarismCases(
        Guid contestId,
        List<Guid> ProblemIds,
        decimal threshold);

    decimal GetSimilarity(
        string code1,
        string code2); // for testing purposes
}
