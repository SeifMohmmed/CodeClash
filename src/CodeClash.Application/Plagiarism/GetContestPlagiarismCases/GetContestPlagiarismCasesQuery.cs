using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Plagiarism.GetContestPlagiarismCases;

public sealed record GetContestPlagiarismCasesQuery(
    Guid ContestId,
    decimal Threshold,
    List<Guid> ProblemIds) : IQuery<GetContestPlagiarismCasesResponse>;
