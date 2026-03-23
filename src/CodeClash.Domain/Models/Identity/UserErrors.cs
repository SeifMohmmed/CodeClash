using CodeClash.Domain.Abstractions;

namespace CodeClash.Domain.Models.Identity;
public static class UserErrors
{
    public static readonly Error NotFound =
        new("User.NotFound", "The user was not found.");

    public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "Invalid email or password.");

    public static readonly Error EmailAlreadyExists =
        new("User.EmailAlreadyExists", "Email is already in use.");

    public static readonly Error UserNameAlreadyExists =
        new("User.UserNameAlreadyExists", "Username is already in use.");
}
