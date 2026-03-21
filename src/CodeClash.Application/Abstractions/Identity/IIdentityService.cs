using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Abstractions.Identity;
public interface IIdentityService
{
    Task<Result<string>> CreateUserAsync(string email, string password);
}
