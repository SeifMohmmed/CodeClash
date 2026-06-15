using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Abstractions.Plagiarism;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Plagiarism.GetCodeSimilarity;

internal sealed class GetCodeSimilarityQueryHandler(
    IPlagiarismService plagiarismService)
    : IQueryHandler<GetCodeSimilarityQuery, decimal>
{
    public Task<Result<decimal>> Handle(
        GetCodeSimilarityQuery request,
        CancellationToken cancellationToken)
    {
        var similarity = plagiarismService.GetSimilarity(request.Code1, request.Code2);
        return Task.FromResult(Result.Success(similarity));
    }
}
