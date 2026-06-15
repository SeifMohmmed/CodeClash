using CodeClash.Application.DTO;
using CodeClash.Domain.Models.Identity;

namespace CodeClash.Application.Abstractions.Token;

public interface ITokenService
{
    Task<AccessTokenDto> GenerateTokensAsync(
        string userId,
        string email,
        CancellationToken cancellationToken = default);

    Task<AccessTokenDto> RotateTokensAsync(
    RefreshToken existingToken,
    CancellationToken cancellationToken = default);
}
