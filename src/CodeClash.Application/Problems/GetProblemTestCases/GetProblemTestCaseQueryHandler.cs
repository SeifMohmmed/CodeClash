using CodeClash.Application.Abstractions.Data;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Premitives;
using Dapper;

namespace CodeClash.Application.Problems.GetProblemTestCases;
internal sealed class GetProblemTestCaseQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetProblemTestCaseQuery, List<TestCaseResponse>>
{
    public async Task<Result<List<TestCaseResponse>>> Handle(
        GetProblemTestCaseQuery request,
        CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
                id AS Id,
                input AS Input,
                output AS Output
            FROM public.testcases
            WHERE problem_id = @ProblemId
            """;

        var testCases = (await connection.QueryAsync<TestCaseResponse>(
            sql,
            new
            {
                request.ProblemId
            })).ToList();

        return testCases;
    }
}
