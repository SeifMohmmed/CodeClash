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
internal sealed class ExecutionService : IExecutionService
{
    // Docker client to communicate with Docker engine
    private readonly DockerClient _dockerClient;

    // Service responsible for file operations (read/write test cases, outputs, etc.)
    private readonly IFileService _fileService;

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
        //memoryFile = Path.Combine(_requestDirectory, "memory_usage.txt");
        // this.unitOfWork = unitOfWork;

        _fileService = fileService;
    }

    /// <summary>
    /// Main method: executes code against multiple test cases.
    /// </summary>
    public async Task<BaseSubmissionResponse> RunCodeAsync(
        string code,
        Language language,
        List<TestCasesDto> testCases,
        decimal runTimeLimit,
        decimal memoryLimit)
    {
        decimal maxRunTime = 0;

        try
        {
            await _fileService.CreateCodeFile(code, language, _requestDirectory);

            // create container 
            await CreateAndStartContainer(language);

            for (int i = 0; i < testCases.Count; i++)
            {
                await _fileService.CreateTestCasesFile(testCases[i].Input, _requestDirectory);
                await ExecuteCodeInContainer(runTimeLimit, memoryLimit);

                // Map DTO -> Domain model before passing
                var testCase = new Testcase
                {
                    Input = testCases[i].Input,
                    Output = testCases[i].Output,
                };

                var result = await CalculateResult(testCase, runTimeLimit, i + 1, testCases[i].Input);

                if (result.SubmissionResult != SubmissionResult.Accepted)
                {
                    return result;
                }

                maxRunTime = Math.Max(maxRunTime, result.ExecutionTime);
            }
        }

        catch (Exception ex)
        {
            throw new Exception("Error while running testcases !!!", ex);
        }

        finally
        {
            if (Directory.Exists(_requestDirectory))
            {
                Directory.Delete(_requestDirectory, true);
            }

            if (_containerId != null)
            {
                await _dockerClient.Containers.RemoveContainerAsync(_containerId, new ContainerRemoveParameters { Force = true });
            }
        }


        return new AcceptedResponse
        {
            ExecutionTime = maxRunTime,
        };
    }

    public async Task<BaseSubmissionResponse> RunCodeAsync(
    string code,
    Language language,
    IEnumerable<Testcase> testCases,  // Changed to support multiple test cases
    decimal runTimeLimit,
    decimal memoryLimit)
    {
        // Create user code file
        await _fileService.CreateCodeFile(code, language, _requestDirectory);

        var testcaseList = testCases.ToList();

        for (int i = 0; i < testcaseList.Count; i++)
        {
            var testcase = testcaseList[i];
            int testcaseNumber = i + 1;

            // Create input file for this test case
            await _fileService.CreateTestCasesFile(testcase.Input, _requestDirectory);

            // Start container fresh per test case
            await CreateAndStartContainer(language);

            // Execute code
            await ExecuteCodeInContainer(runTimeLimit, memoryLimit);

            // Evaluate result — return early on any failure
            var result = await CalculateResult(testcase, runTimeLimit, testcaseNumber, testcase.Input);

            if (result.SubmissionResult != SubmissionResult.Accepted)
            {
                return result;
            }
        }

        // All test cases passed
        return new AcceptedResponse
        {
            ExecutionMemory = 3m,
            NumberOfPassedTestCases = testcaseList.Count,
            // ExecutionTime: ideally sum or max across runs — placeholder here
            ExecutionTime = 0m
        };
    }


    /// <summary>
    /// Reads execution outputs and determines result (AC, WA, TLE, etc.)
    /// </summary>
    private async Task<BaseSubmissionResponse> CalculateResult(
        Testcase testCase,
        decimal runTimeLimit,
        int testcaseNumber,
        string input)
    {
        string output = await _fileService.ReadFileAsync(outputFile);
        string error = await _fileService.ReadFileAsync(errorFile);
        string runTime = await _fileService.ReadFileAsync(runTimeFile);
        string runTimeError = await _fileService.ReadFileAsync(runTimeErrorFile);

        // Initialize the run result
        // BaseSubmissionResponse response = default;


        if (!string.IsNullOrEmpty(error))
        {
            return new CompilationErrorResponse
            {
                Message = error,
                SubmissionResult = SubmissionResult.CompilationError,
                ExecutionTime = 0m

            };
        }
        if (!string.IsNullOrEmpty(runTimeError))
        {
            return new RunTimeErrorResponse
            {
                Message = runTimeError,
                SubmissionResult = SubmissionResult.RunTimeError,
                NumberOfPassedTestCases = testcaseNumber - 1,
                ExecutionTime = 0,
                Input = input
            };
        }
        if (runTime?.Contains("TIMELIMITEXCEEDED") == true)
        {
            return new TimeLimitExceedResponse
            {
                NumberOfPassedTestCases = testcaseNumber - 1,
                ExecutionTime = runTimeLimit,
                SubmissionResult = SubmissionResult.TimeLimitExceeded,
                Input = input
            };
        }

        if (output.TrimEnd('\n') != testCase.Output.TrimEnd('\n'))
        {
            return new WrongAnswerResponse
            {
                NumberOfPassedTestCases = testcaseNumber - 1,
                ActualOutput = output,
                ExpectedOutput = testCase.Output,
                SubmissionResult = SubmissionResult.WrongAnswer,
                ExecutionTime = Helper.ExtractExecutionTime(runTime!),
                Input = input
            };
        }


        return new AcceptedResponse
        {
            ExecutionTime = Helper.ExtractExecutionTime(runTime!),
            ExecutionMemory = 3m,
        };
    }

    private async Task<BaseSubmissionResponse> CalculateResult(
        CustomTestcaseDto testcaseDto,
        decimal runTimeLimit)
    {
        string output = await _fileService.ReadFileAsync(outputFile);
        string error = await _fileService.ReadFileAsync(errorFile);
        string runTime = await _fileService.ReadFileAsync(runTimeFile);
        string runTimeError = await _fileService.ReadFileAsync(runTimeErrorFile);

        if (!string.IsNullOrEmpty(error))
        {
            return new CompilationErrorResponse
            {
                Message = error,
                SubmissionResult = SubmissionResult.CompilationError,
                ExecutionTime = 0m
            };
        }

        if (!string.IsNullOrEmpty(runTimeError))
        {
            return new RunTimeErrorResponse
            {
                Message = runTimeError,
                SubmissionResult = SubmissionResult.RunTimeError,
                ExecutionTime = Helper.ExtractExecutionTime(runTime ?? string.Empty)
            };
        }

        if (runTime?.Contains("TIMELIMITEXCEEDED") == true)
        {
            return new TimeLimitExceedResponse
            {
                ExecutionTime = runTimeLimit,
                SubmissionResult = SubmissionResult.TimeLimitExceeded,
            };
        }

        if (output.TrimEnd('\n') != testcaseDto.ExpectedOutput.TrimEnd('\n'))
        {
            return new WrongAnswerResponse
            {
                ActualOutput = output,
                ExpectedOutput = testcaseDto.ExpectedOutput,
                SubmissionResult = SubmissionResult.WrongAnswer,
                ExecutionTime = Helper.ExtractExecutionTime(runTime ?? string.Empty)
            };
        }

        return new AcceptedResponse
        {
            ExecutionTime = Helper.ExtractExecutionTime(runTime ?? string.Empty),
            ExecutionMemory = 3m,
        };
    }

    /// <summary>
    /// Creates and starts Docker container with correct compiler image
    /// </summary>
    private async Task CreateAndStartContainer(
        Language language)
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
    private async Task ExecuteCodeInContainer(
        decimal timeLimit,
        decimal memoryLimit)
    {
        string command = Helper.CreateExecuteCodeCommand(
            _containerId,
            timeLimit,
            memoryLimit);

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
}
