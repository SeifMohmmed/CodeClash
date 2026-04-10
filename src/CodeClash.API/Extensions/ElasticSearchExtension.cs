using CodeClash.Application.Abstractions.ElasticSearch;

namespace CodeClash.API.Extensions;

public static class ElasticSearchExtension
{
    public static async Task InitializeElasticSearchAsync(
        this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var elasticSearch = scope.ServiceProvider
            .GetRequiredService<IElasticService>();

        await elasticSearch.InitializeIndexes();
    }
}
