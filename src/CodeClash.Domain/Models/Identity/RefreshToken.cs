using Microsoft.AspNetCore.Identity;

namespace CodeClash.Domain.Models.Identity;
/// <summary>
/// Represents refresh token stored in database.
/// Used to issue new access tokens.
/// </summary>
public class RefreshToken
{
    public Guid Id { get; set; }   // Primary key.
    public required string UserId { get; set; } // Associated user identifier.
    public required string Token { get; set; } // Refresh token value.
    public required DateTime ExpireAtUtc { get; set; } // Expiration date in UTC.
    public IdentityUser User { get; set; }  // Navigation property to IdentityUser.
}
