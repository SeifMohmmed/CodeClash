using CodeClash.Domain.Premitives.Responses.ElasticSearchResponses;

namespace CodeClash.Application.Abstractions.ElasticSearch;
public interface IElasticService
{
    Task InitializeIndexes();
    Task<bool> IndexDocumentAsync<T>(
        T document,
        string indexName) where T : class;

    Task<bool> BulkIndexDocumentsAsync<T>(
        IEnumerable<T> documents,
        string indexName) where T : class;

    Task<IEnumerable<ProblemDocument>> SearchProblemsAsync(
        string? searchText,
        List<int>? topicsIds,
        int? difficulty);

    Task<IEnumerable<ProblemDocument>> SearchProblemsAsync(
        string searchText);

    Task<IEnumerable<BlogDocument>> SearchBlogsAsync(
        string searchText,
        List<string> tags,
        string indexName);

    Task<bool> UpdateDocumentAsync<T>(
        T document,
        string documentId,
        string indexName) where T : class;

    Task<bool> DeleteDocumentAsync(
        string documentId,
        string indexName);

    Task<T?> GetDocumentByIdAsync<T>(
        string documentId,
        string indexName) where T : class;

    Task<bool> IndexExistsAsync(string indexName);

    Task<bool> CreateIndexIfNotExistsAsync(string indexName);

    Task<bool> DeleteIndexAsync(string indexName);
}
