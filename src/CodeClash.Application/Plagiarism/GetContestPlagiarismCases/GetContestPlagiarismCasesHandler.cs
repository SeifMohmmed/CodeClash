using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Abstractions.Plagiarism;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Plagiarism.GetContestPlagiarismCases;

internal sealed class GetContestPlagiarismCasesHandler(
    IPlagiarismService plagiarismService) : IQueryHandler<GetContestPlagiarismCasesQuery, GetContestPlagiarismCasesResponse>
{
    public async Task<Result<GetContestPlagiarismCasesResponse>> Handle(
        GetContestPlagiarismCasesQuery request,
        CancellationToken cancellationToken)
    {
        var cases = (await plagiarismService.GetPlagiarismCases(
            request.ContestId,
            request.ProblemIds,
            request.Threshold)).ToList();

        return Result.Success(new GetContestPlagiarismCasesResponse(
            request.ContestId,
            request.Threshold,
            request.ProblemIds,
            cases));
    }
}
