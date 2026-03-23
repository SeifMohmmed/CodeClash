using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CodeClash.Application.Abstractions.Identity;
using CodeClash.Application.DTO;
using CodeClash.Application.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace CodeClash.Infrastructure.Implementation;

/// <summary>
/// Responsible for generating JWT access tokens and refresh tokens.
/// Uses JwtAuthOptions configuration for signing and expiration.
/// </summary>
internal sealed class TokenProvider(IOptions<JwtAuthOptions> options) : ITokenProvider
{
    // Strongly typed JWT configuration
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    /// <summary>
    /// Creates both access token and refresh token.
    /// </summary>
    public AccessTokenDto Create(TokenRequest tokenRequest)
    {
        return new AccessTokenDto(
            GenerateAccessToken(tokenRequest),
            GenerateRefreshToken());
    }

    /// <summary>
    /// Generates signed JWT access token.
    /// </summary>
    private string GenerateAccessToken(TokenRequest tokenRequest)
    {
        // Create symmetric security key from configured secret
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtAuthOptions.Key));

        // Signing credentials using HMAC SHA256 algorithm
        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        // Claims included inside token payload
        List<Claim> claims =
        [
            // Subject → unique user identifier
            new (JwtRegisteredClaimNames.Sub, tokenRequest.UserId),

            // User email
            new (JwtRegisteredClaimNames.Email, tokenRequest.Email),
        ];
        // Token descriptor contains metadata + signing info
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            // Token expiration
            Expires = DateTime.UtcNow.AddMinutes(
                _jwtAuthOptions.ExpirationInMinutes),
            // Token issuer
            Issuer = _jwtAuthOptions.Issuer,
            // Intended audience
            Audience = _jwtAuthOptions.Audience,
            SigningCredentials = credentials
        };

        // Handler responsible for creating JWT
        var handler = new JsonWebTokenHandler();
        // Generate token string
        string accessToken = handler.CreateToken(tokenDescriptor);

        return accessToken;
    }

    /// <summary>
    /// Generates a cryptographically secure refresh token.
    /// </summary>
    private string GenerateRefreshToken()
    {
        // Generate secure random bytes
        byte[] randomBytes = RandomNumberGenerator.GetBytes(32);

        // Convert to Base64 string for storage/transmission
        return Convert.ToBase64String(randomBytes);
    }

}
