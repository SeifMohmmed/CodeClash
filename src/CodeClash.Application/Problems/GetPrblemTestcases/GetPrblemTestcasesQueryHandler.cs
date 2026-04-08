using CodeClash.Application.Abstractions.Data;
using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using Dapper;

namespace CodeClash.Application.Problems.GetPrblemTestcases;
internal sealed class GetPrblemTestcasesQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetTestCaseQuery, List<TestCaseResponse>>
{
    public async Task<Result<List<TestCaseResponse>>> Handle(
        GetTestCaseQuery request,
        CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string checkSql = """
            SELECT COUNT(1)
            FROM code_clash.problems
            WHERE id = @ProblemId
            """;

        var problemExists = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(
                checkSql,
                new { request.ProblemId },
                cancellationToken: cancellationToken));

        if (!problemExists)
        {
            return Result.Failure<List<TestCaseResponse>>(
                new Error("Problem.NotFound", "Problem was not found."));
        }

        const string sql = """
            SELECT
                id AS Id,
                input AS Input,
                output AS Output
            FROM code_clash.testcases
            WHERE problem_id = @ProblemId
            """;

        var testCases = (await connection.QueryAsync<TestCaseResponse>(
            sql,
            new
            {
                request.ProblemId
            })).ToList();

        return testCases ?? [];
    }
}
