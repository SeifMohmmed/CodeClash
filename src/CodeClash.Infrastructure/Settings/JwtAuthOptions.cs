namespace CodeClash.Infrastructure.Settings;
/// <summary>
/// Represents JWT authentication configuration settings.
/// Bound from configuration (appsettings.json → Jwt section).
/// </summary>
public sealed class JwtAuthOptions
{
    public string Issuer { get; init; } // Token issuer (authorization server).
    public string Audience { get; init; } // Token audience (API consumers).
    public string Key { get; init; }  // Secret key used to sign tokens.
    public int ExpirationInMinutes { get; init; }// Access token lifetime in minutes.
    public int RefreshTokenExiprationDays { get; init; } // Refresh token lifetime in days.
}
