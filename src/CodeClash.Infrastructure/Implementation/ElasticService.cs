using CodeClash.Application.Abstractions.ElasticSearch;
using CodeClash.Application.Helpers;
using CodeClash.Domain.Premitives;
using CodeClash.Domain.Premitives.Responses.ElasticSearchResponses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;

namespace CodeClash.Infrastructure.Implementation;
internal sealed class ElasticService : IElasticService
{
    private readonly ElasticClient _client;
    private readonly ElasticSettings _elasticSettings;
    private readonly ILogger<ElasticService> _logger;
    //private readonly HttpClient _httpClient;
    public ElasticService(
        IOptions<ElasticSettings> options,
        ILogger<ElasticService> logger)
    {
        _elasticSettings = options.Value;
        _logger = logger;

#pragma warning disable CA2000
        var settings = new ConnectionSettings(new Uri(_elasticSettings.Url))
            .DefaultIndex(_elasticSettings.DefaultIndex);

        _client = new ElasticClient(settings);
        //_httpClient = httpClient;
    }

    // ─── Index Management ────────────────────────────────────────────────────
    public async Task<bool> IndexExistsAsync(
    string indexName)
    {
        var response = await _client.Indices.ExistsAsync(indexName);
        return response.Exists;
    }

    public async Task<bool> CreateIndexIfNotExistsAsync(
    string indexName)
    {
        var exists = await IndexExistsAsync(indexName);
        if (exists)
        {
            return true;
        }

        var response = await _client.Indices.CreateAsync(indexName, c => c
            .Map(m => m.AutoMap()));

        if (!response.IsValid)
        {
            _logger.LogError(
                "Failed to create index '{IndexName}': {Error}",
                indexName, response.OriginalException?.Message);
        }

        return response.IsValid;
    }

    public async Task<bool> DeleteIndexAsync(
        string indexName)
    {
        var response = await _client.Indices.DeleteAsync(indexName);

        if (!response.IsValid)
        {
            _logger.LogError(
                "Failed to delete index '{IndexName}': {Error}",
                indexName, response.OriginalException?.Message);
        }

        return response.IsValid;
    }

    public async Task InitializeIndexes()
    {
        await EnsureIndexAsync<ProblemDocument>(_elasticSettings.DefaultIndex);
        await EnsureIndexAsync<BlogDocument>(_elasticSettings.SecondaryIndex);
    }

    private async Task EnsureIndexAsync<T>(string indexName) where T : class
    {
        var existsResponse = await _client.Indices.ExistsAsync(indexName);
        if (existsResponse.Exists)
        {
            return;
        }

        var createResponse = await _client.Indices.CreateAsync(indexName, c => c
            .Map<T>(m => m.AutoMap()));

        if (!createResponse.IsValid)
        {
            _logger.LogError(
                "Failed to initialize index '{IndexName}': {Error}",
                indexName, createResponse.OriginalException?.Message);
        }
        else
        {
            _logger.LogInformation("Index '{IndexName}' created successfully.", indexName);
        }
    }


    // ─── Single Document Operations ──────────────────────────────────────────

    public async Task<bool> IndexDocumentAsync<T>(
        T document,
        string indexName) where T : class
    {
        var response = await _client.IndexAsync(document, i => i.Index(indexName));

        if (!response.IsValid)
        {
            _logger.LogError("Failed to index document: {Error}", response.OriginalException?.Message);
        }

        return response.IsValid;
    }

    public async Task<T?> GetDocumentByIdAsync<T>(
    string documentId,
    string indexName) where T : class
    {
        var response = await _client.GetAsync<T>(documentId, g => g.Index(indexName));

        if (!response.IsValid && response.OriginalException is not null)
        {
            _logger.LogError(
                "Failed to get document '{DocumentId}' from '{IndexName}': {Error}",
                documentId, indexName, response.OriginalException.Message);
        }

        return response.Found ? response.Source : null;
    }

    public async Task<bool> UpdateDocumentAsync<T>(
    T document,
    string documentId,
    string indexName) where T : class
    {
        var response = await _client.UpdateAsync<T>(documentId, u => u
            .Index(indexName)
            .Doc(document));

        if (!response.IsValid)
        {
            _logger.LogError(
                "Failed to update document '{DocumentId}' in '{IndexName}': {Error}",
                documentId, indexName, response.OriginalException?.Message);
        }

        return response.IsValid;
    }

    public async Task<bool> DeleteDocumentAsync(
    string documentId,
    string indexName)
    {
        var response = await _client.DeleteAsync<object>(documentId, d => d.Index(indexName));


        if (!response.IsValid)
        {
            _logger.LogError(
                "Failed to delete document '{DocumentId}' from '{IndexName}': {Error}",
                documentId, indexName, response.OriginalException?.Message);
        }

        return response.IsValid;
    }

    // ─── Bulk Operations ─────────────────────────────────────────────────────
    public async Task<bool> BulkIndexDocumentsAsync<T>(
    IEnumerable<T> documents,
    string indexName) where T : class
    {
        var documentList = documents.ToList();
        if (documentList.Count == 0)
        {
            _logger.LogWarning("BulkIndexDocumentsAsync called with empty documents list for index '{IndexName}'.", indexName);
            return true;
        }

        var response = await _client.BulkAsync(b => b
            .Index(indexName)
            .IndexMany(documentList));

        if (!response.IsValid)
        {
            _logger.LogError(
                "Bulk index failed for '{IndexName}': {Error}",
                indexName, response.OriginalException?.Message);
            return false;
        }

        if (response.Errors)
        {
            var reasons = response.ItemsWithErrors
                .Select(e => e.Error?.Reason)
                .Where(r => r is not null);

            _logger.LogWarning(
                "Bulk index for '{IndexName}' had partial failures: {Errors}",
                indexName, string.Join(" | ", reasons));
        }

        return !response.Errors;
    }

    // ─── Search ──────────────────────────────────────────────────────────────

    public async Task<IEnumerable<BlogDocument>> SearchBlogsAsync(
        string searchText,
        List<string> tags,
        string indexName)
    {
        var response = await _client.SearchAsync<BlogDocument>(s => s
    .Index(indexName)
    .Query(q => q
        .Bool(b => b
            .Must(
                m => m.Match(mq => mq.Field("content").Query(searchText)),
                m => m.Terms(tq => tq.Field("tags").Terms(tags))
            )
        )
    )
);

        return response.Documents;
    }

    public async Task<IEnumerable<ProblemDocument>> SearchProblemsAsync(
        string? searchText,
        List<int>? topicsIds,
        Difficulty? difficulty,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var response = await _client.SearchAsync<ProblemDocument>(s => s
                             .Index(ElasticSearchIndexes.Problems)
                             .From((pageNumber - 1) * pageSize)
                             .Size(pageSize)
                             .Query(q => q
                                 .Bool(b => b
                                     .Must(
                                         m => m.Match(mq => mq
                                             .Field(f => f.Name) // Fuzzy search on Name
                                             .Query(searchText)
                                             .Fuzziness(Nest.Fuzziness.EditDistance(2))
                                         )
                                     )
                                     .Filter(
                                         f => f.Terms(t => t
                                             .Field(ff => ff.Topics) // Exact match on topics
                                             .Terms(topicsIds)
                                         ),
                                         f => f.Term(t => t
                                             .Field(ff => ff.Difficulty) // Exact match on difficulty
                                             .Value(difficulty)
                                         )
                                     )
                                 )
                             )
                         );

        if (!response.IsValid)
        {
            _logger.LogError(
                "Problem search failed: {Error}",
                response.OriginalException?.Message);
            return Enumerable.Empty<ProblemDocument>();
        }

        return response.Hits.Select(hit => hit.Source);
    }

    public async Task<IEnumerable<ProblemDocument>> SearchProblemsAsync(
        string searchText)
    {
        var response = await _client.SearchAsync<ProblemDocument>(s => s
            .Index(ElasticSearchIndexes.Problems)
            .Query(q => q
                .Bool(b => b
                    .Must(
                        m => m.Match(mq => mq
                            .Field(f => f.Name)
                            .Query(searchText)
                            .Fuzziness(Fuzziness.EditDistance(2))
                        )
                    )
                )
            )
        );

        if (!response.IsValid)
        {
            _logger.LogError(
                "Problem search failed: {Error}",
                response.OriginalException?.Message);
            return Enumerable.Empty<ProblemDocument>();
        }

        return response.Hits.Select(h => h.Source);
    }
}
