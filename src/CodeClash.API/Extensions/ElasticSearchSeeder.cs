using CodeClash.Domain.Premitives;
using CodeClash.Domain.Premitives.Responses.ElasticSearchResponses;
using CodeClash.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace CodeClash.API.Extensions;

public sealed class ElasticSearchSeeder(
    IElasticClient elasticClient,
    ApplicationDbContext dbContext,
        ILogger<ElasticSearchSeeder> logger)
{
    public async Task SeedAsync()
    {
        // Skip if already seeded
        var count = await elasticClient.CountAsync<ProblemDocument>(c => c
            .Index(ElasticSearchIndexes.Problems)
        );

        if (count.Count > 0)
        {
            logger.LogInformation("ES index already seeded, skipping.");
            return;
        }

        // Pull from DB including topics junction table
        var problems = await dbContext.Problems
            .Include(p => p.ProblemTopics)
            .AsNoTracking()
            .ToListAsync();

        if (!problems.Any())
        {
            logger.LogWarning("No problems in database to seed.");
            return;
        }

        // Map to ES document
        var documents = problems.Select(p => new ProblemDocument
        {
            Id = p.Id,
            Name = p.Name,
            Difficulty = p.Difficulty,
            Topics = p.ProblemTopics
                            .Select(pt => pt.TopicId)  // adjust property name
                            .ToList()
        }).ToList();

        // Bulk index
        var bulkResponse = await elasticClient.BulkAsync(b => b
            .Index(ElasticSearchIndexes.Problems)
            .IndexMany(documents)
        );

        if (bulkResponse.Errors)
        {
            foreach (var item in bulkResponse.ItemsWithErrors)
            {
                Console.WriteLine($"Failed to index doc: {item.Error.Reason}");
            }

            return;
        }

        Console.WriteLine($"Seeded {documents.Count} problems into Elasticsearch.");
    }
}
