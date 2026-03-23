using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Identity;

namespace CodeClash.Infrastructure.Repositories;
internal sealed class RefreshTokenRepository(
    ApplicationIdentityDbContext identityDbContext) : IRefreshTokenRepository
{
    public async Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default)
    {
        await identityDbContext.AddAsync(refreshToken, cancellationToken);
    }
}
