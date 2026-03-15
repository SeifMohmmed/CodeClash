using CodeClash.Application.Authentication.Login;
using CodeClash.Application.Authentication.Register;
using CodeClash.Domain.Models.Identity;

namespace CodeClash.Application.Mapping;
public static class UserMappings
{
    public static ApplicationUser ToApplicationUser(
        this RegisterCommand command)
    {
        return new ApplicationUser
        {
            Email = command.Email,
            UserName = command.UserName
        };
    }

    public static ApplicationUser ToApplicationUser(
        this LoginQuery query)
    {
        return new ApplicationUser
        {
            Email = query.Email,
            UserName = query.Email
        };
    }
}
