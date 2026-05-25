using CodeClash.Application.Abstractions.Execution;
using CodeClash.Application.Abstractions.File;
using CodeClash.Application.DTO;
using CodeClash.Domain.Models.TestCases;
using CodeClash.Domain.Premitives;
using CodeClash.Domain.Premitives.Responses;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace CodeClash.Infrastructure.Implementation;

/// <summary>
/// Service responsible for executing user code inside Docker containers
/// and evaluating results against test cases.
/// </summary>
internal sealed class ExecutionService : IExecutionService, IDisposable
{
    // Docker client to communicate with Docker engine
    private readonly DockerClient _dockerClient;

    // Service responsible for file operations (read/write test cases, outputs, etc.)
    private readonly IFileService _fileService;

    // Temporary directory per execution request
    private readonly string _requestDirectory;

    // Docker container ID used for execution
    private string? _containerId;

    // Paths for execution artifacts
    private readonly string _outputFile;
    private readonly string _errorFile;
    private readonly string _runTimeFile;
    private readonly string _runTimeErrorFile;

    // Command to keep container alive (idle)
    internal static readonly string[] parameters = new[] { "tail", "-f", "/dev/null" };

    public ExecutionService(IFileService fileService)
    {
        // Initialize Docker client (Windows named pipe)
        var config = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine"));
        _dockerClient = config.CreateClient();
        config.Dispose();

        // Create unique temp directory for this execution
        _requestDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_requestDirectory);

        // Define files used for communication with container
        _outputFile = Path.Combine(_requestDirectory, "output.txt");
        _errorFile = Path.Combine(_requestDirectory, "error.txt");
        _runTimeFile = Path.Combine(_requestDirectory, "runtime.txt");
        _runTimeErrorFile = Path.Combine(_requestDirectory, "runtime_errors.txt");

        _fileService = fileService;
    }

    /// <summary>
    /// Overload that accepts DTOs — maps to domain model and delegates.
    /// </summary>
    public async Task<BaseSubmissionResponse> RunCodeAsync(
        string code,
        Language language,
        List<TestCasesDto> testCases,
        decimal runTimeLimit)
    {
        var domainTestCases = testCases
            .Select(t => new Testcase { Input = t.Input, Output = t.Output })
            .ToList();

        return await RunCodeAsync(code, language, domainTestCases, runTimeLimit);
    }

    /// <summary>
    /// Main method: executes code against multiple test cases.
    /// </summary>
    public async Task<BaseSubmissionResponse> RunCodeAsync(
        string code,
        Language language,
        List<Testcase> testCases,
        decimal runTimeLimit)
    {
        decimal maxRunTime = 0m;

        try
        {
            await _fileService.CreateCodeFile(code, language, _requestDirectory);
            await CreateAndStartContainer(language);

            for (int i = 0; i < testCases.Count; i++)
            {
                var testcase = testCases[i];
                int testcaseNumber = i + 1;

                await _fileService.CreateTestCasesFile(testcase.Input, _requestDirectory);
                await ExecuteCodeInContainer(runTimeLimit);

                var result = await CalculateResult(testcase, testcaseNumber, testCases.Count);

                if (result.SubmissionResult != SubmissionResult.Accepted)
                {
                    return result;
                }

                if (result is AcceptedResponse accepted)
                {
                    maxRunTime = Math.Max(maxRunTime, accepted.ExecutionTime);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error while running testcases.", ex);
        }
        finally
        {
            await CleanupAsync();
        }

        return new AcceptedResponse
        {
            ExecutionTime = maxRunTime,
            NumberOfPassedTestCases = testCases.Count,
            TotalTestcases = testCases.Count
        };
    }

    /// <summary>
    /// Reads execution outputs and determines result (AC, WA, TLE, CE, RTE).
    /// </summary>
    private async Task<BaseSubmissionResponse> CalculateResult(
        Testcase testCase,
        int testcaseNumber,
        int totalTestcases)
    {
        var (output, error, runTime, runTimeError) = await ReadExecutionOutputsAsync();

        if (!string.IsNullOrEmpty(error))
        {
            return new CompilationErrorResponse
            {
                Message = error,
                SubmissionResult = SubmissionResult.CompilationError,
                NumberOfPassedTestCases = 0,
                TotalTestcases = totalTestcases
            };
        }

        if (!string.IsNullOrEmpty(runTimeError))
        {
            return new RunTimeErrorResponse
            {
                Message = runTimeError,
                SubmissionResult = SubmissionResult.RunTimeError,
                Input = testCase.Input,
                TotalTestcases = totalTestcases,
                NumberOfPassedTestCases = testcaseNumber - 1,
                ExpectedOutput = testCase.Output
            };
        }

        if (runTime?.Contains("TIMELIMITEXCEEDED") == true)
        {
            return new TimeLimitExceedResponse
            {
                Input = testCase.Input,
                NumberOfPassedTestCases = testcaseNumber - 1,
                TotalTestcases = totalTestcases,
                SubmissionResult = SubmissionResult.TimeLimitExceeded,
                ExpectedOutput = testCase.Output
            };
        }

        if (output.TrimEnd('\n') != testCase.Output.TrimEnd('\n'))
        {
            return new WrongAnswerResponse
            {
                NumberOfPassedTestCases = testcaseNumber - 1,
                TotalTestcases = totalTestcases,
                ActualOutput = output,
                Input = testCase.Input,
                ExpectedOutput = testCase.Output,
                SubmissionResult = SubmissionResult.WrongAnswer,
            };
        }

        return new AcceptedResponse
        {
            NumberOfPassedTestCases = testcaseNumber,
            ExecutionTime = Helper.ExtractExecutionTime(runTime!),
            TotalTestcases = totalTestcases
        };
    }

    /// <summary>
    /// Overload for custom test cases (no expected output comparison).
    /// </summary>
    private async Task<BaseSubmissionResponse> CalculateResult(
        CustomTestcaseDto testcaseDto,
        int testcaseNumber,
        int totalTestcases)
    {
        var (output, error, runTime, runTimeError) = await ReadExecutionOutputsAsync();

        if (!string.IsNullOrEmpty(error))
        {
            return new CompilationErrorResponse
            {
                Message = error,
                SubmissionResult = SubmissionResult.CompilationError,
                NumberOfPassedTestCases = 0,
                TotalTestcases = totalTestcases
            };
        }

        if (!string.IsNullOrEmpty(runTimeError))
        {
            return new RunTimeErrorResponse
            {
                Message = runTimeError,
                SubmissionResult = SubmissionResult.RunTimeError,
                TotalTestcases = totalTestcases,
                NumberOfPassedTestCases = testcaseNumber - 1,
                ExpectedOutput = testcaseDto.ExpectedOutput
            };
        }

        if (runTime?.Contains("TIMELIMITEXCEEDED") == true)
        {
            return new TimeLimitExceedResponse
            {
                Input = testcaseDto.Input,
                NumberOfPassedTestCases = testcaseNumber - 1,
                TotalTestcases = totalTestcases,
                SubmissionResult = SubmissionResult.TimeLimitExceeded,
                ExpectedOutput = testcaseDto.ExpectedOutput
            };
        }

        if (output.TrimEnd('\n') != testcaseDto.ExpectedOutput.TrimEnd('\n'))
        {
            return new WrongAnswerResponse
            {
                NumberOfPassedTestCases = testcaseNumber - 1,
                TotalTestcases = totalTestcases,
                ActualOutput = output,
                Input = testcaseDto.Input,
                ExpectedOutput = testcaseDto.ExpectedOutput,
                SubmissionResult = SubmissionResult.WrongAnswer,
            };
        }

        return new AcceptedResponse
        {
            NumberOfPassedTestCases = testcaseNumber,
            ExecutionTime = Helper.ExtractExecutionTime(runTime ?? string.Empty),
            TotalTestcases = totalTestcases
        };
    }

    /// <summary>
    /// Reads all execution output files in one place — shared by both CalculateResult overloads.
    /// </summary>
    private async Task<(string output, string error, string runTime, string runTimeError)> ReadExecutionOutputsAsync()
    {
        return (
            await _fileService.ReadFileAsync(_outputFile),
            await _fileService.ReadFileAsync(_errorFile),
            await _fileService.ReadFileAsync(_runTimeFile),
            await _fileService.ReadFileAsync(_runTimeErrorFile)
        );
    }

    /// <summary>
    /// Creates and starts Docker container with the correct compiler image.
    /// </summary>
    private async Task CreateAndStartContainer(Language language)
    {
        var image = language switch
        {
            Language.py => Helper.PythonCompiler,
            Language.cpp => Helper.CppCompiler,
            Language.cs => Helper.CSharpCompiler,
            _ => throw new ArgumentException("Unsupported language")
        };

        var createContainerResponse = await _dockerClient.Containers.CreateContainerAsync(
            new CreateContainerParameters
            {
                HostConfig = new HostConfig
                {
                    Binds = new[]
                    {
                        $"{_requestDirectory}:/code",
                        $"{Helper.ScriptFilePath}:/run_code.sh"
                    },
                    NetworkMode = "bridge",         // isolated bridge network (allows loopback, blocks external)
                    Memory = 256 * 1024 * 1024,     // limit memory to 256 MB
                    AutoRemove = false
                },
                Name = "code_container",
                Image = image,
                Cmd = parameters,                   // keep container alive
            });

        _containerId = createContainerResponse.ID;

        await _dockerClient.Containers.StartContainerAsync(
            _containerId,
            new ContainerStartParameters());
    }

    /// <summary>
    /// Executes code inside the container using a shell command.
    /// </summary>
    private async Task ExecuteCodeInContainer(decimal timeLimit)
    {
        string command = Helper.CreateExecuteCodeCommand(_containerId!, timeLimit);

        using var process = new System.Diagnostics.Process();

        process.StartInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/C {command}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            process.Start();
            await process.WaitForExitAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error while executing client code.", ex);
        }
    }

    /// <summary>
    /// Cleans up temp directory and removes the Docker container.
    /// </summary>
    private async Task CleanupAsync()
    {
        if (Directory.Exists(_requestDirectory))
        {
            Directory.Delete(_requestDirectory, true);
        }

        if (!string.IsNullOrEmpty(_containerId))
        {
            await _dockerClient.Containers.RemoveContainerAsync(
                _containerId,
                new ContainerRemoveParameters { Force = true });
        }
    }

    /// <summary>
    /// Disposes the Docker client.
    /// </summary>
    public void Dispose() => _dockerClient.Dispose();
}
