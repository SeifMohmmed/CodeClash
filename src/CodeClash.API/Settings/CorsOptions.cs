namespace CodeClash.API.Settings;

public sealed class CorsOptions
{
    public const string SectionName = "Cors";
    public const string PolicyName = "CodeClashCorsPolicy";

    public required string[] AllowedOrigins { get; init; }
}
