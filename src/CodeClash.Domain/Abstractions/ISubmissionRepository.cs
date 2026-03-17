using CodeClash.Domain.Models.Submits;

namespace CodeClash.Domain.Abstractions;
public interface ISubmissionRepository
{
    IQueryable<Submit> GetAllSubmissions(Guid problemId, string userId);
}
