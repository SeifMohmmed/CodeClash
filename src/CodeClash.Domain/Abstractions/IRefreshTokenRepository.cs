using CodeClash.Domain.Models.Identity;

namespace CodeClash.Domain.Abstractions;
public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);

}
