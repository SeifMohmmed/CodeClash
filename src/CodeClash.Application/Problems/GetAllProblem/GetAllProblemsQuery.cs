using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Problems.GetAllProblem;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Problems.GetAll;
public record GetAllProblemsQuery(
    string? UserId,
    List<int>? TopicsIds,
    string? Name,
    Difficulty? Difficulty) : IQuery<IEnumerable<GetAllProblemResponse>>;
