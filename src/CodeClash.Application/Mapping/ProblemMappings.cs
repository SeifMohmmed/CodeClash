using CodeClash.Application.Problems.CreateProblem;
using CodeClash.Domain.Models;

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
            Rate = command.Rate
        };
    }
}
