using CodeClash.Application.Problems.CreateProblem;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Models.Submits;
using CodeClash.Domain.Models.TestCases;
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
            SetterId = command.ProblemSetterId,

            // Convert string -> enum
            Difficulty = Enum.Parse<Difficulty>(command.Difficulty, true),

            RunTimeLimit = command.RunTimeLimit,
            MemoryLimit = command.MemoryLimit,

            Name = string.Empty,
            Description = string.Empty,
            ContestPoints = ContestPoints.Level1,

            Images = new List<ProblemImage>(),
            Testcases = new List<Testcase>(),
            ProblemTopics = new List<ProblemTopic>(),
            Submissions = new List<Submit>()
        };
    }
}
