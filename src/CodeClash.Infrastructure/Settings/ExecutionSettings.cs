namespace CodeClash.Infrastructure.Settings;

public sealed class ExecutionSettings
{
    public string ScriptFilePath { get; set; } = string.Empty;
    public string DockerEndpoint { get; set; } = string.Empty;
}
