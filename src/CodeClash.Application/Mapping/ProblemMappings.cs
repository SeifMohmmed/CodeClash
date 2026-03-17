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
            Name = command.Name,
            Description = command.Description,
            Difficulty = (Difficulty)command.Rate,

            RunTimeLimit = 1,
            MemoryLimit = 256

        };
    }
}
