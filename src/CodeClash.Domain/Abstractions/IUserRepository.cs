using CodeClash.Domain.Models.Identity;

namespace CodeClash.Domain.Abstractions;
public interface IUserRepository
{
    Task AddAsync(User user);

    Task<User?> GetByIdentityIdAsync(string identityId);

}
