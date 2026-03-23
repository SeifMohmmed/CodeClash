namespace CodeClash.Application.DTO;
public sealed record RefreshTokenDto
{
    public required string RefreshToken { get; init; }  // Refresh token string.
}
