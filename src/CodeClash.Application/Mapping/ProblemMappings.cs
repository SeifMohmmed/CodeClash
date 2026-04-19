using CodeClash.Application.DTO;
using CodeClash.Application.Problems.CreateProblem;
using CodeClash.Application.Problems.GetAllProblem;
using CodeClash.Application.Problems.GetProblemById;
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

    public static GetProblemByIdResponse ToDetailsResponse(this Problem problem)
    {
        return new GetProblemByIdResponse
        {
            Name = problem.Name,
            Description = problem.Description,
            Difficulty = problem.Difficulty,

            // TestCases mapping
            TasteCases = problem.Testcases?
                .Select(tc => new TestCasesDto
                {
                    Input = tc.Input,
                    Output = tc.Output
                })
                .ToList() ?? new List<TestCasesDto>(),

            // Topics mapping
            Topics = problem.ProblemTopics?
                .Select(pt => new TopicDto
                {
                    Id = pt.Topic.Id,
                    Name = pt.Topic.Name
                })
                .ToList() ?? new List<TopicDto>(),

            // Stats (example logic)
            Submissions = problem.Submissions?.Count ?? 0,

            Accepted = problem.Submissions?
                .Count(s => s.Result == SubmissionResult.Accepted) ?? 0
        };
    }

}
