namespace CodeClash.Infrastructure;

/// <summary>
/// Contains database schema names used across the application.
/// Centralizing schema names avoids hardcoding strings.
/// </summary>
internal static class Schemas
{
    /// <summary>
    /// Main application schema.
    /// All EF tables will be created inside this schema.
    /// </summary>
    public const string Application = "code_clash";
    public const string Identity = "identity";
}
