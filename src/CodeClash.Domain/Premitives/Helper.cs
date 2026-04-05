using System.Globalization;
using System.Text.RegularExpressions;

namespace CodeClash.Domain.Premitives;
public static class Helper
{
    public const string ScriptFilePath = @"D:\\Computer Science\\Web API Projects\\CodeClash\\src\\CodeClash.Domain\\Premitives\\run_code.sh";
    public const string PythonCompiler = "python:3.8-slim";
    public const string CppCompiler = "gcc:latest";
    public const string CSharpCompiler = "mcr.microsoft.com/dotnet/sdk:5.0";

    //public static string ExecuteCodeCommand(string containerId, decimal runTime)
    //{
    //    string runTimeLimit = $"{runTime}s";
    //    // Prepare the docker exec command to run the script inside the container
    //    return $"docker exec {containerId} /usr/bin/bash /run_code.sh {runTimeLimit}";
    //}

    public static string CreateExecuteCodeCommand(
        string containerId,
        decimal timeLimit,
        decimal memoryLimit)
    {
        string runTimeLimit = $"{timeLimit}s";

        string runMemoryLimit = $"{Math.Round(memoryLimit)}mb";

        return $"docker exec {containerId} /usr/bin/bash /run_code.sh {runTimeLimit} {runMemoryLimit}";
    }

    public static decimal ExtractExecutionTime(string time)
    {
        Match match = Regex.Match(time, @"real\t\d+m([\d.]+)s");

        string seconds = match.Groups[1].Value;

        return decimal.Parse(seconds, CultureInfo.InvariantCulture);
    }
}
