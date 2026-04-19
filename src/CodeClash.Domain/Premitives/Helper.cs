using System.Globalization;
using System.Text.RegularExpressions;

namespace CodeClash.Domain.Premitives;
public static class Helper
{
    public static readonly string ScriptFilePath = SetScriptFilePath();
    public const string PythonCompiler = "python:3.8-slim";
    public const string CppCompiler = "gcc:latest";
    public const string CSharpCompiler = "mcr.microsoft.com/dotnet/sdk:5.0";

    //public static string ExecuteCodeCommand(string containerId, decimal runTime)
    //{
    //    string runTimeLimit = $"{runTime}s";
    //    // Prepare the docker exec command to run the script inside the container
    //    return $"docker exec {containerId} /usr/bin/bash /run_code.sh {runTimeLimit}";
    //}

    public static string SetScriptFilePath()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        var directoryInfo = new DirectoryInfo(currentDirectory);

        // Navigate up to the solution root
        while (directoryInfo != null && !DirectoryContainsFile(directoryInfo.FullName, "*.sln"))
        {
            directoryInfo = directoryInfo.Parent;
        }

        if (directoryInfo is null)
        {
            throw new Exception("Solution root not found.");
        }

        // Combine solution path with the Infrastructure project path
        string infrastructurePath = Path.Combine(
            directoryInfo.FullName, "CodeClash.Domain", "Premitives", "run_code.sh");


        return infrastructurePath;
    }

    private static bool DirectoryContainsFile(
        string directoryPath,
        string searchPattern)
    {
        return Directory.GetFiles(directoryPath, searchPattern).Length > 0;
    }

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
