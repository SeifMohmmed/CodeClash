using CodeClash.Application.Abstractions.Messaging;
using CodeClash.Application.Problems.GetAllProblem;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Problems.GetAll;
public record GetAllProblemsQuery(
    List<Guid>? Topics,
    string? Name,
    Difficulty? Difficulty,
    ProblemStatus? Status,
    SortBy SortBy,
    Order Order,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<PagedResult<GetAllProblemResponse>>;
