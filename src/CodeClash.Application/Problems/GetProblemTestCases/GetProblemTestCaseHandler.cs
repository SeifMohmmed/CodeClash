using CodeClash.Application.Abstractions.Data;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Premitives;
using Dapper;

namespace CodeClash.Application.Problems.GetProblemTestCases;
internal sealed class GetProblemTestCaseHandler(
    ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetProblemTestCase, TestCaseResponse>
{
    public async Task<Result<TestCaseResponse>> Handle(
        GetProblemTestCase request,
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

        var problem = await connection.QueryFirstOrDefaultAsync<TestCaseResponse>(
            sql,
            new
            {
                request.ProblemId
            });

        return problem;
    }
}
