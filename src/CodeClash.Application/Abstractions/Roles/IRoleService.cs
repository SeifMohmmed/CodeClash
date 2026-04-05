using CodeClash.Domain.Models.Roles;
namespace CodeClash.Application.Abstractions.Roles;
public interface IRoleService
{
    Task<Role> CreateRoleAsync(string roleName);
    Task<Role> GetRoleByNameAsync(string name);
    Task<IEnumerable<Role>> GetAllRolesAsync();
    Task AssignRoleAsync(string userId, string roleName);

}
