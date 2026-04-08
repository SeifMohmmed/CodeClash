using CodeClash.Application.Problems.CreateProblem;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;

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
            Difficulty = Enum.Parse<Difficulty>(command.Difficulty, true),
            RunTimeLimit = command.RunTimeLimit,
            MemoryLimit = command.MemoryLimit,
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
}
