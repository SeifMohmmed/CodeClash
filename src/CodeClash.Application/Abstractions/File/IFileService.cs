using CodeClash.Domain.Premitives;
using Microsoft.AspNetCore.Http;

namespace CodeClash.Application.Abstractions.File;
/// <summary>
/// Handles file operations (validation, creation, reading).
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Validates if uploaded file extension matches selected language.
    /// </summary>
    bool CheckFileExtension(
        IFormFile file,
        Language language);


    /// <summary>
    /// Reads content from a file.
    /// </summary>
    Task<string> ReadFileAsync(
        string filePath);

    Task<string> ReadFile(
    IFormFile filePath);


    /// <summary>
    /// Creates a file containing test case input.
    /// </summary>
    Task<string> CreateTestCasesFile(
        string testCase,
        string requestDirectory);


    /// <summary>
    /// Creates source code file inside temp directory.
    /// </summary>
    Task<string> CreateCodeFile(
        string code,
        Language language,
        string requestDirectory);
}
