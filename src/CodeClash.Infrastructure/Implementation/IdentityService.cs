using CodeClash.Application.Abstractions.Identity;
using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Premitives;
using Microsoft.AspNetCore.Identity;

namespace CodeClash.Infrastructure.Implementation;
internal sealed class IdentityService(
    UserManager<IdentityUser> userManager) : IIdentityService
{
    public async Task<Result<string>> CreateUserAsync(
        string email,
        string password)
    {
        var user = new IdentityUser
        {
            Email = email,
            UserName = email
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var error = MapIdentityError(result);

            return Result.Failure<string>(error);
        }

        return Result.Success<string>(user.Id);
    }



    private static Error MapIdentityError(IdentityResult result)
    {
        if (result.Errors.Any(e =>
            e.Code == "DuplicateEmail" ||
            e.Code == "DuplicateUserName"))
        {
            return IdentityErrors.EmailAlreadyExists;
        }

        if (result.Errors.Any(e => e.Code == "InvalidUserName"))
        {
            return IdentityErrors.InvalidCredentials;
        }

        return IdentityErrors.Unknown;
    }

}

public static class IdentityErrors
{
    public static readonly Error EmailAlreadyExists =
        new("Auth.EmailExists", "Email already exists");

    public static readonly Error InvalidCredentials =
        new("Auth.InvalidCredentials", "Invalid credentials");

    public static readonly Error Unknown =
        new("Auth.Unknown", "Unknown identity error");
}
