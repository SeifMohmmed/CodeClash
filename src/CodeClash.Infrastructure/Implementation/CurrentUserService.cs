using System.Security.Claims;
using CodeClash.Application.Abstractions.CurrentUser;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Identity;
using Microsoft.AspNetCore.Http;

namespace CodeClash.Infrastructure.Implementation;
internal sealed class CurrentUserService(
    IHttpContextAccessor contextAccessor,
    IUserRepository userRepository) : ICurrentUserService
{
    public string? IdentityId =>
        contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

    public Task<User?> GetUserAsync()
    {
        var identityId = IdentityId;
        return identityId is null
            ? Task.FromResult<User?>(null)
            : userRepository.GetByIdentityIdAsync(identityId);
    }
}
