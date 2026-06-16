using Bogus;
using CodeClash.Application.Abstractions.ElasticSearch;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;
using CodeClash.Domain.Premitives.Responses.ElasticSearchResponses;
using CodeClash.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.API.Extensions;

public static class SeedDataExtension
{
    public static async Task SeedDataAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var existingCount = await dbContext.Problems.CountAsync();

        if (existingCount == 0)
        {
            await SeedSqlAsync(dbContext);
        }

        await SeedElasticAsync(scope, dbContext);
    }

    private static async Task SeedSqlAsync(ApplicationDbContext dbContext)
    {
        var faker = new Faker();

        // ── Users ────────────────────────────────────────────────────────────
        var users = new Faker<User>()
            .RuleFor(u => u.Id, _ => Guid.NewGuid().ToString())
            .RuleFor(u => u.IdentityId, _ => Guid.NewGuid().ToString())
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Rating, f => (short)f.Random.Int(800, 2400))
            .RuleFor(u => u.RankName, f => f.PickRandom<UserStatus>())
            .RuleFor(u => u.ImagePath, f => f.Image.LoremFlickrUrl(200, 200, "person"))
            .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
            .RuleFor(u => u.CreatedAtUtc, _ => DateTime.UtcNow)
            .RuleFor(u => u.UpdatedAtUtc, _ => DateTime.UtcNow)
            .Generate(20);

        await dbContext.Users.AddRangeAsync(users);
        await dbContext.SaveChangesAsync();

        // ── Contests ─────────────────────────────────────────────────────────
        var userIds = users.Select(u => u.Id).ToList();

        var contests = Enumerable.Range(0, 10).Select(_ =>
        {
            var start = faker.Date.Future();
            return new Contest
            {
                Name = faker.Company.CatchPhrase(),
                SetterId = faker.PickRandom(userIds),
                StartDate = start,
                EndDate = start.AddHours(faker.Random.Int(2, 5)),
                BlogId = null
            };
        }).ToList();

        await dbContext.Contests.AddRangeAsync(contests);
        await dbContext.SaveChangesAsync();

        // ── Problems ─────────────────────────────────────────────────────────
        var contestIds = contests.Select(c => c.Id).ToList();

        var problems = Enumerable.Range(0, 100).Select(_ => new Problem
        {
            Name = faker.Lorem.Sentence(3),
            SetterId = faker.PickRandom(userIds),
            ContestId = faker.PickRandom(contestIds),
            Difficulty = faker.PickRandom<Difficulty>(),
            ContestPoints = faker.PickRandom<ContestPoints>(),
            Description = faker.Lorem.Paragraphs(3),
            RunTimeLimit = faker.Random.Decimal(1, 3),
            MemoryLimit = faker.Random.Decimal(128, 512),
            BlogId = null
        }).ToList();

        await dbContext.Problems.AddRangeAsync(problems);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedElasticAsync(
        IServiceScope scope,
        ApplicationDbContext dbContext)
    {
        var elasticService = scope.ServiceProvider.GetRequiredService<IElasticService>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILogger<ElasticSearchSeeder>>();

        var existing = await elasticService.SearchProblemsAsync(string.Empty);
        if (existing.Any())
        {
            logger.LogInformation("ES problems index already has data, skipping.");
            return;
        }

        var dbProblems = await dbContext.Problems
            .Select(p => new { p.Id, p.Name, p.Difficulty })
            .ToListAsync();

        if (!dbProblems.Any())
        {
            logger.LogWarning("No problems found in DB to seed into ES.");
            return;
        }

        var documents = dbProblems.Select(p => new ProblemDocument
        {
            Id = p.Id,
            Name = p.Name,
            Difficulty = p.Difficulty,
            Topics = []
        }).ToList();

        var success = await elasticService.BulkIndexDocumentsAsync(
            documents,
            ElasticSearchIndexes.Problems);

        if (success)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Seeded {Count} problems into Elasticsearch.", documents.Count);
            }
        }
        else
        {
            logger.LogError("Failed to bulk index problems into Elasticsearch.");
        }
    }
}
