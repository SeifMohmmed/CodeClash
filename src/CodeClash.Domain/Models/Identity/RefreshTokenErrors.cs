using CodeClash.Domain.Abstractions;

namespace CodeClash.Domain.Models.Identity;
public static class RefreshTokenErrors
{
    public static readonly Error Invalid =
        new("RefreshToken.Invalid", "Invalid refresh token");

    public static readonly Error Expired =
        new("RefreshToken.Expired", "Refresh token expired");
}
