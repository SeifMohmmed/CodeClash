using CodeClash.Application.Abstractions.File;
using CodeClash.Domain.Premitives;
using Microsoft.AspNetCore.Http;

namespace CodeClash.Infrastructure.Implementation;
/// <summary>
/// Implementation of file operations used during code execution.
/// </summary>
internal sealed class FileService : IFileService
{
    /// <summary>
    /// Checks if uploaded file extension matches selected language.
    /// </summary>
    public bool CheckFileExtension(IFormFile file, Language language)
    {
        var extension = System.IO.Path.GetExtension(file.FileName);

        // If no extension → invalid
        if (extension is null)
        {
            return false;
        }

        // Remove '.' from extension (e.g. ".cs" → "cs")
        extension = extension[1..];

        bool found = false;

        // Compare extension with language enum
        foreach (Language lan in Enum.GetValues<Language>())
        {
            if (lan == language && extension == lan.ToString())
            {
                found = true;
            }
        }

        return found;
    }

    /// <summary>
    /// Creates source code file inside temp directory.
    /// </summary>
    public async Task<string> CreateCodeFile(string code, Language language, string requestDirectory)
    {
        // Example: main.cs / main.py / main.cpp
        string filePath = Path.Combine(requestDirectory, $"main.{language}");

        await System.IO.File.WriteAllTextAsync(filePath, code);

        return filePath;
    }

    /// <summary>
    /// Creates file containing test case input.
    /// </summary>
    public async Task<string> CreateTestCasesFile(string testCase, string requestDirectory)
    {
        string filePath = Path.Combine(requestDirectory, "testcases.txt");

        await System.IO.File.WriteAllTextAsync(filePath, testCase);

        return filePath;
    }

    public async Task<string> ReadFile(
        IFormFile filePath)
    {
        string content;
        using (var reader = new StreamReader(filePath.OpenReadStream()))
        {
            content = await reader.ReadToEndAsync();
        }
        return content;
    }

    /// <summary>
    /// Reads file content asynchronously.
    /// </summary>
    public async Task<string> ReadFileAsync(string filePath)
        => await System.IO.File.ReadAllTextAsync(filePath);
}
