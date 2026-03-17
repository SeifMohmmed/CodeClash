using System.Diagnostics;
using CodeClash.Application.Abstractions.Execution;
using CodeClash.Application.Abstractions.File;
using CodeClash.Domain.Models.TestCases;
using CodeClash.Domain.Premitives;
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
    private readonly IFileService fileService;

    // Temporary directory per execution request
    private readonly string _requestDirectory;

    // Docker container ID used for execution
    private string _containerId;

    // Paths for execution artifacts
    private readonly string outputFile;
    private readonly string errorFile;
    private readonly string runTimeFile;
    private readonly string runTimeErrorFile;

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
        outputFile = Path.Combine(_requestDirectory, "output.txt");
        errorFile = Path.Combine(_requestDirectory, "error.txt");
        runTimeFile = Path.Combine(_requestDirectory, "runtime.txt");
        runTimeErrorFile = Path.Combine(_requestDirectory, "runtime_errors.txt");

        this.fileService = fileService;
    }

    /// <summary>
    /// Main method: executes code against multiple test cases.
    /// </summary>
    public async Task<List<TestCaseRunResult>> ExecuteCodeAsync(
        string code,
        Language language,
        List<Testcase> testCases,
        decimal runTimeLimit)
    {
        var results = new List<TestCaseRunResult>();

        try
        {
            // Create and start Docker container
            await CreateAndStartContainer(language);

            for (int i = 0; i < testCases.Count; i++)
            {
                // Normalize escaped new lines
                testCases[i].Input = testCases[i].Input.Replace("\\n", "\n");
                testCases[i].Output = testCases[i].Output.Replace("\\n", "\n");

                // Write input to file (mounted inside container)
                await fileService.CreateTestCasesFile(testCases[i].Input, _requestDirectory);

                // Execute user code inside container
                await ExecuteCodeInContainer(runTimeLimit);

                // Evaluate result (output vs expected)
                var result = await CalculateResult(testCases[i], runTimeLimit);

                results.Add(result);

                // Early exit if any test case fails
                if (result.Result != SubmissionResult.Accepted)
                {
                    return results;
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error while running testcases: {ex.Message}", ex);
        }
        finally
        {
            // Cleanup temp directory
            if (Directory.Exists(_requestDirectory))
            {
                Directory.Delete(_requestDirectory, true);
            }

            // Remove container
            if (_containerId != null)
            {
                await _dockerClient.Containers.RemoveContainerAsync(
                    _containerId,
                    new ContainerRemoveParameters { Force = true });
            }
        }

        return results;
    }

    /// <summary>
    /// Reads execution outputs and determines result (AC, WA, TLE, etc.)
    /// </summary>
    private async Task<TestCaseRunResult> CalculateResult(Testcase testCase, decimal runTimeLimit)
    {
        // Read generated files
        string output = await fileService.ReadFileAsync(outputFile);
        string error = await fileService.ReadFileAsync(errorFile);
        string runTime = await fileService.ReadFileAsync(runTimeFile);
        string runTimeError = await fileService.ReadFileAsync(runTimeErrorFile);

        // Initialize result object
        var runResult = new TestCaseRunResult
        {
            TestCaseId = testCase.Id,
            Input = testCase.Input,
            ExpectedOutput = testCase.Output,
        };

        // Compilation error
        if (!string.IsNullOrEmpty(error))
        {
            return SetResult(runResult, error, SubmissionResult.CompilationError);
        }

        // Runtime error
        if (!string.IsNullOrEmpty(runTimeError))
        {
            return SetResult(runResult, runTimeError, SubmissionResult.RunTimeError);
        }

        // Time limit exceeded
        if (runTime?.Contains("TIMELIMITEXCEEDED") == true)
        {
            runResult.Result = SubmissionResult.TimeLimitExceeded;
            runResult.RunTime = runTimeLimit;
            return runResult;
        }

        // Assign output
        runResult.ActualOutput = output;
        runResult.RunTime = 1m; // TO=DO: Replace with actual runtime measurement

        // Compare expected vs actual
        runResult.Result = testCase.Output != runResult.ActualOutput
            ? SubmissionResult.WrongAnswer
            : SubmissionResult.Accepted;

        return runResult;
    }

    /// <summary>
    /// Helper to set error-based result
    /// </summary>
    private TestCaseRunResult SetResult(TestCaseRunResult runResult, string error, SubmissionResult result)
    {
        runResult.Error = error;
        runResult.Result = result;
        return runResult;
    }

    /// <summary>
    /// Creates and starts Docker container with correct compiler image
    /// </summary>
    private async Task CreateAndStartContainer(Language language)
    {
        // Select image based on language
        var image = language switch
        {
            Language.py => Helper.PythonCompiler,
            Language.cpp => Helper.CppCompiler,
            Language.cs => Helper.CSharpCompiler,
            _ => throw new ArgumentException("Unsupported language")
        };

        // Create container
        var createContainerResponse = await _dockerClient.Containers.CreateContainerAsync(
            new CreateContainerParameters
            {
                HostConfig = new HostConfig
                {
                    // Mount temp directory and execution script
                    Binds = new[]
                    {
                        $"{_requestDirectory}:/code",
                        $"{Helper.ScriptFilePath}:/run_code.sh"
                    },
                    NetworkMode = "none", // disable internet access (security)
                    Memory = 256 * 1024 * 1024, // limit memory
                    AutoRemove = false
                },
                Name = "code_container",
                Image = image,
                Cmd = parameters, // keep container alive
            });

        _containerId = createContainerResponse.ID;

        // Start container
        await _dockerClient.Containers.StartContainerAsync(
            _containerId,
            new ContainerStartParameters());
    }

    /// <summary>
    /// Executes code inside container using shell command
    /// </summary>
    private async Task ExecuteCodeInContainer(decimal runTime)
    {
        string command = Helper.ExecuteCodeCommand(_containerId, runTime);

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

            // Wait until execution finishes
            await process.WaitForExitAsync();
        }
        catch (Exception)
        {
            throw new Exception("Error While Executing Client Code !!");
        }
    }

    public void Dispose()
    {
        _dockerClient?.Dispose();
    }

    /// <summary>
    /// Custom async wait for process exit (not used currently)
    /// </summary>
    private Task<bool> WaitForExitAsync(Process process)
    {
        var tcs = new TaskCompletionSource<bool>();

        process.EnableRaisingEvents = true;
        process.Exited += (sender, args) => tcs.TrySetResult(true);

        if (process.HasExited)
        {
            tcs.TrySetResult(true);
        }

        return tcs.Task;
    }
}
