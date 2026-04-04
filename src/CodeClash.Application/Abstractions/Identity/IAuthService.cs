using CodeClash.Domain.Premitives;
using Microsoft.AspNetCore.Identity;

namespace CodeClash.Application.Abstractions.Identity;
public interface IAuthService
{
    Task<Result<string>> CreateUserAsync(string email, string password);

    Task<IdentityUser?> GetUserByEmailAsync(string email);

    Task<IdentityUser?> GetUserByIdAsync(string userId);

    Task<string> GenerateEmailConfirmationTokenAsync(IdentityUser user);

    Task<Result<string>> ConfirmEmailAsync(string userId, string token);

    Task<bool> CheckPasswordAsync(IdentityUser user, string password);
}
