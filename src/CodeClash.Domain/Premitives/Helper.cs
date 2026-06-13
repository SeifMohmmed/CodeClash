using System.Text.Json;
using System.Text.RegularExpressions;
using CodeClash.Domain.Requests;
using Microsoft.AspNetCore.Http;

namespace CodeClash.Domain.Premitives;

public static class Helper
{
    // Lazy — only resolved when actually needed for code execution
    private static readonly Lazy<string> _scriptFilePath = new(ResolveScriptFilePath);
    public static string ScriptFilePath => _scriptFilePath.Value;

    public const string PythonCompiler = "python:3.8-slim";
    public const string CppCompiler = "gcc:latest";
    public const string CSharpCompiler = "mcr.microsoft.com/dotnet/sdk:5.0";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static string ResolveScriptFilePath()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        var directoryInfo = new DirectoryInfo(currentDirectory);

        while (directoryInfo != null && !DirectoryContainsFile(directoryInfo.FullName, "*.sln"))
        {
            directoryInfo = directoryInfo.Parent;
        }

        if (directoryInfo is null)
        {
            throw new Exception("Solution root not found.");
        }

        return Path.Combine(
            directoryInfo.FullName,
            "CodeClash.Domain",
            "Premitives",
            "run_code.sh");
    }

    public static T DeserializeObject<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }

    public static IEnumerable<T> DeserializeCollection<T>(string json)
    {
        return JsonSerializer.Deserialize<IEnumerable<T>>(json, JsonOptions)
            ?? Enumerable.Empty<T>();
    }

    private static bool DirectoryContainsFile(
        string directoryPath,
        string searchPattern)
    {
        return Directory.GetFiles(directoryPath, searchPattern).Length > 0;
    }

    public static string CreateExecuteCodeCommand(
        string containerId,
        decimal timeLimit)
    {
        string runTimeLimit = $"{timeLimit}s";

        // string runMemoryLimit = $"{Math.Round(memoryLimit)}mb";

        return $"docker exec {containerId} /usr/bin/bash /run_code.sh {runTimeLimit}";
    }

    public static decimal ExtractExecutionTime(string time)
    {
        Match match = Regex.Match(time, @"real\t\d+m([\d.]+)s");

        string seconds = match.Groups[1].Value;

        if (decimal.TryParse(seconds, out decimal result))
        {
            return result;
        }
        else
        {
            return 0;
        }
    }

    public static decimal ExtractExecutionMemory(string memory)
    {
        // the string will be like "Memory Usage: 12345 KB"

        // get the number part in rgex 
        Match match = Regex.Match(memory, @"\d+");
        string memoryInKB = match.Value;

        if (decimal.TryParse(memoryInKB, out decimal result))
        {
            return result;
        }
        else
        {
            return 0;
        }
    }

    public static bool ValidateFile(
      Language language,
      double maxSizeInMb,
      double minSizeInMb,
      IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);
        var requiredExtension = '.' + language.ToString();

        if (!extension.Equals(requiredExtension, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var sizeInBytes = file.Length;
        var minBytes = minSizeInMb * 1024 * 1024;
        var maxBytes = maxSizeInMb * 1024 * 1024;

        if (sizeInBytes < minBytes || sizeInBytes > maxBytes)
        {
            return false;
        }

        return true;
    }

    public static string GenerateContestKey(Guid contestId)
        => $"contest:{contestId}:standing";

    public static string GenerateContestProblemsKey(Guid contestId)
    => $"contest-problems:{contestId}";

    public static string GenerateUserSubmissionKey(string userId, Guid contestId)
        => $"user:{userId}:contest:{contestId}:submissions";

    public static string ConvertUserToRedisMemeber(UserToCache user)
    => JsonSerializer.Serialize(user);

    public static UserToCache ConvertRedisMemberToUser(string member)
    => JsonSerializer.Deserialize<UserToCache>(member)!;
}
