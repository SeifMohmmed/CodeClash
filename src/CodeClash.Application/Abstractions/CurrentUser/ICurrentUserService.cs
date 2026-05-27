using CodeClash.Domain.Models.Identity;

namespace CodeClash.Application.Abstractions.CurrentUser;
public interface ICurrentUserService
{
    string? IdentityId { get; }
    Task<User?> GetUserAsync();
}
