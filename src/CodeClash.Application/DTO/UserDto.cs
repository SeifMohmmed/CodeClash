namespace CodeClash.Application.DTO;
public sealed record UserDto
{
    public required string Id { get; init; }
    public required string Email { get; init; }
    public required string Name { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }

    public string? ImagePath { get; init; }
    public short Rating { get; init; }
    public string RankName { get; init; } = default!;
}
