namespace CodeClash.Application.RunCode;

public sealed record RunCodeResponse(
    string Input,
    string Output,
    bool Passed);
