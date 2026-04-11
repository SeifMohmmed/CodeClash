using CodeClash.Application.Problems.CreateProblem;
using CodeClash.Application.Problems.GetAllProblem;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;
using CodeClash.Domain.Premitives.Responses.ElasticSearchResponses;

namespace CodeClash.Application.Mapping;
public static class ProblemMappings
{
    public static Problem ToEntity(
        this CreateProblemCommand command)
    {
        return new Problem
        {
            ContestId = command.ContestId,
            Name = command.Name,
            Description = command.Description,
            SetterId = command.SetterId,
            Difficulty = command.Difficulty,
            RunTimeLimit = command.RunTimeLimit,
            MemoryLimit = (decimal)command.MemoryLimit,
            ContestPoints = ContestPoints.Level1,
            Images = [],
            Testcases = [],
            ProblemTopics = [],
            Submissions = []
        };
    }
    public static CreateProblemResponse ToResponse(this Problem problem)
    {
        return new CreateProblemResponse(
            problem.Id,
            problem.Name,
            problem.Difficulty.ToString());
    }

    public static GetAllProblemResponse ToGetAllResponse(this ProblemDocument problem)
    {
        return new GetAllProblemResponse
        {
            Name = problem.Name ?? string.Empty,
            Difficulty = problem.Difficulty,
            Topics = problem.Topics ?? new List<Guid>(),
            IsSolved = false  // Set by the handler after submission check
        };
    }
}
