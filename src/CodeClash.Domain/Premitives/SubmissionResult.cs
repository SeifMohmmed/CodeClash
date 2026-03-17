namespace CodeClash.Domain.Premitives;
public enum SubmissionResult
{
    Pending,
    Accepted,
    WrongAnswer,
    TimeLimitExceeded,
    MemoryLimitExceeded,
    CompilationError,
    RunTimeError
}
