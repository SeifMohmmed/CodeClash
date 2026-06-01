using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Contests.GetAllContests;

internal sealed class GetAllContestsQueryHandler(
    IContestRepository contestRepository)
    : IQueryHandler<GetAllContestsQuery, IReadOnlyList<GetAllContestsResponse>>
{
    public async Task<Result<IReadOnlyList<GetAllContestsResponse>>> Handle(
        GetAllContestsQuery request,
        CancellationToken cancellationToken)
    {
        var contests = await contestRepository.GetAllAsync(cancellationToken);

        IReadOnlyList<GetAllContestsResponse> response = contests
            .Select(c => c.ToGetAllContestsResponse(request.UserId))
            .ToList();

        return Result.Success(response);
    }
}
