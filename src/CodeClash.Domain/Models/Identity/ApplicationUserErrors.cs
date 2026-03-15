using CodeClash.Domain.Abstractions;

namespace CodeClash.Domain.Models.Identity;
public static class ApplicationUserErrors
{
    public static readonly Error NotFound =
        new("ApplicationUser.NotFound", "The user was not found.");

    public static readonly Error InvalidCredentials =
        new("ApplicationUser.InvalidCredentials", "Invalid email or password.");

    public static readonly Error EmailAlreadyExists =
        new("ApplicationUser.EmailAlreadyExists", "Email is already in use.");

    public static readonly Error UserNameAlreadyExists =
        new("ApplicationUser.UserNameAlreadyExists", "Username is already in use.");
}
