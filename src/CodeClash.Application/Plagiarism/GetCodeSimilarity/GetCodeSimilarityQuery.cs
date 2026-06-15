using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Plagiarism.GetCodeSimilarity;

public sealed record GetCodeSimilarityQuery(
    string Code1,
    string Code2) : IQuery<decimal>;
