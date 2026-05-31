using CodeClash.Application.DTO;

namespace CodeClash.Application.Plagiarism.GetContestPlagiarismCases;

public sealed record GetContestPlagiarismCasesResponse(
    Guid ContestId,
    decimal Threshold,
    List<Guid> ProblemIds,
    List<PlagiarismCaseDto> Cases);
