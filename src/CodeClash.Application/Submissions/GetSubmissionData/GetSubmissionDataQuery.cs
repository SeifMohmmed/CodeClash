using CodeClash.Application.Abstractions.Messaging;

namespace CodeClash.Application.Submissions.GetSubmissionData;
public record GetSubmissionDataQuery(
    Guid SubmissionId) : IQuery<GetSubmissionDataResponse>;
