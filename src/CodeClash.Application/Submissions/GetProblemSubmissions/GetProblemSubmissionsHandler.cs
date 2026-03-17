using CodeClash.Application.Abstractions.Data;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Premitives;
using Dapper;

namespace CodeClash.Application.Submissions.GetProblemSubmissions;
internal sealed class GetProblemSubmissionsHandler(
    ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetProblemSubmissions, IReadOnlyList<GetProblemSubmissionsResponse>>
{
    public async Task<Result<IReadOnlyList<GetProblemSubmissionsResponse>>> Handle(
        GetProblemSubmissions request,
        CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """

            """;

        var submissions = await connection.QueryAsync<GetProblemSubmissionsResponse>(
            sql,
            new
            {
                request.ProblemId
            }
            );

        return submissions.ToList();
    }
}
