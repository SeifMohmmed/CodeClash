using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Problems.GetAllProblem;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Problems.GetAll;
public record GetAllProblemsQuery(
    List<int>? TopicsIds,
    string? Name,
    Difficulty? Difficulty) : IQuery<IEnumerable<GetAllProblemResponse>>;
