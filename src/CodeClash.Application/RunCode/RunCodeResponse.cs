namespace CodeClash.Application.RunCode;
internal sealed record RunCodeResponse(
    string Input,
    string Output,
    bool Passed);
