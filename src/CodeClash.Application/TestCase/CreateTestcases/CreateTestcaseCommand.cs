using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.TestCase.CreateTestcases;
public sealed record CreateTestcaseCommand(
 Guid ProblemId,
 string Input,
 string Output) : ICommand<Guid>;
