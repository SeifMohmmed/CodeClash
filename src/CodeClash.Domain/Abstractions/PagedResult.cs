namespace CodeClash.Domain.Abstractions;

public sealed record PagedResult<T>(IEnumerable<T> Items, int TotalPages);
