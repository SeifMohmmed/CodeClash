using System.Data;
using Bogus;
using CodeClash.Application.Abstractions.Data;
using CodeClash.Application.Abstractions.ElasticSearch;
using CodeClash.Domain.Premitives;
using CodeClash.Domain.Premitives.Responses.ElasticSearchResponses;
using Dapper;

namespace CodeClash.API.Extensions;

public static class SeedDataExtension
{
    /// <summary>
    /// Extension method used to seed the database with fake apartment data.
    /// It runs once at application startup to populate the database for testing/demo purposes.
    /// </summary>
    public static async Task SeedDataAsync(this IApplicationBuilder app)
    {
        // Create a scoped lifetime to resolve services
        using var scope = app.ApplicationServices.CreateScope();

        // Resolve SQL connection factory
        var sqlConnectionFactory = scope.ServiceProvider
            .GetRequiredService<ISqlConnectionFactory>();

        // Open DB connection
        using var connection = sqlConnectionFactory.CreateConnection();

        // Check if problems table already has data
        var existingCount = await connection
            .ExecuteScalarAsync<int>("SELECT COUNT(*) FROM code_clash.problems");

        // Seed SQL only if database is empty
        if (existingCount == 0)
        {
            await SeedSqlAsync(connection);
        }

        // Always attempt to seed Elasticsearch
        await SeedElasticAsync(scope, connection);
    }

    /// <summary>
    /// Seeds relational database (Users, Contests, Problems)
    /// </summary>
    private static async Task SeedSqlAsync(IDbConnection connection)
    {
        var faker = new Faker();

        //--------------------------------
        // Generate Users
        //--------------------------------

        // Define fake user rules
        var users = new Faker<SeedUser>()
            .RuleFor(u => u.Id, _ => Guid.NewGuid())
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Rating, f => f.Random.Int(800, 2400))
            .RuleFor(u => u.RankName, f => f.Random.Int(0, 5))
            .RuleFor(u => u.ImagePath, f => f.Image.LoremFlickrUrl(200, 200, "person"))
            .RuleFor(u => u.Gender, f => f.Random.Int(0, 1))
            .RuleFor(u => u.CreatedAtUtc, _ => DateTime.UtcNow)
            .RuleFor(u => u.UpdatedAtUtc, _ => DateTime.UtcNow)
            .RuleFor(u => u.IdentityId, _ => Guid.NewGuid());

        var userList = users.Generate(20);

        // SQL insert query for users
        const string userSql = """
            INSERT INTO code_clash.users
            (id, rank_name, image_path, rating, gender, name, email,
             created_at_utc, updated_at_utc, identity_id)
            VALUES
            (@Id, @RankName, @ImagePath, @Rating, @Gender, @Name, @Email,
             @CreatedAtUtc, @UpdatedAtUtc, @IdentityId);
            """;

        // Execute batch insert
        await connection.ExecuteAsync(userSql, userList);

        //--------------------------------
        // Generate Contests
        //--------------------------------

        // Get all user IDs to assign as contest setters
        var userIds = (await connection
            .QueryAsync<string>("SELECT id FROM code_clash.users"))
            .ToList();

        var contests = new List<object>();
        for (int i = 0; i < 10; i++)
        {
            var start = faker.Date.Future();
            var end = start.AddHours(faker.Random.Int(2, 5));
            contests.Add(new
            {
                Id = Guid.NewGuid(),
                Name = faker.Company.CatchPhrase(),
                SetterId = faker.PickRandom(userIds),
                StartDate = start,
                EndDate = end,
                BlogId = (Guid?)null
            });
        }

        // SQL insert query for contests
        const string contestSql = """
            INSERT INTO code_clash.contests
            (id, name, setter_id, start_date, end_date, blog_id)
            VALUES
            (@Id, @Name, @SetterId, @StartDate, @EndDate, @BlogId);
            """;

        await connection.ExecuteAsync(contestSql, contests);

        //--------------------------------
        // Generate Problems
        //--------------------------------

        // Get contest IDs for assigning problems
        var contestIds = (await connection
            .QueryAsync<Guid>("SELECT id FROM code_clash.contests"))
            .ToList();

        var problems = new List<object>();

        for (int i = 0; i < 100; i++)
        {
            problems.Add(new
            {
                Id = Guid.NewGuid(),
                Name = faker.Lorem.Sentence(3),
                SetterId = faker.PickRandom(userIds),
                ContestId = faker.PickRandom(contestIds),
                Difficulty = faker.Random.Int(0, 2),
                ContestPoints = faker.Random.Int(100, 1000),
                Description = faker.Lorem.Paragraphs(3),
                RunTimeLimit = faker.Random.Decimal(1, 3),
                MemoryLimit = faker.Random.Decimal(128, 512),
                BlogId = (Guid?)null
            });
        }

        // SQL insert query for problems
        const string problemSql = """
            INSERT INTO code_clash.problems
            (id, name, setter_id, contest_id, difficulty,
             contest_points, description, run_time_limit, memory_limit, blog_id)
            VALUES
            (@Id, @Name, @SetterId, @ContestId, @Difficulty,
             @ContestPoints, @Description, @RunTimeLimit, @MemoryLimit, @BlogId);
            """;

        await connection.ExecuteAsync(problemSql, problems);
    }

    /// <summary>
    /// Seeds Elasticsearch index with problem documents
    /// </summary>
    private static async Task SeedElasticAsync(
        IServiceScope scope,
        IDbConnection connection)
    {
        // Resolve Elastic service and logger
        var elasticService = scope.ServiceProvider.GetRequiredService<IElasticService>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILogger<ElasticSearchSeeder>>();

        // Check if Elasticsearch already contains documents
        var problems = await elasticService.SearchProblemsAsync(string.Empty);
        var hasDocuments = problems.Any();

        if (hasDocuments)
        {
            logger.LogInformation("ES problems index already has data, skipping.");
            return;
        }

        // Fetch problems from database
        var dbProblems = (await connection.QueryAsync<(Guid Id, string Name, int Difficulty)>(
            "SELECT id, name, difficulty FROM code_clash.problems"
        )).ToList();

        if (!dbProblems.Any())
        {
            logger.LogWarning("No problems found in DB to seed into ES.");
            return;
        }

        // Map DB entities to Elasticsearch documents
        var documents = dbProblems.Select(p => new ProblemDocument
        {
            Id = p.Id,
            Name = p.Name,
            Difficulty = p.Difficulty,
            Topics = []
        }).ToList();

        // Bulk index documents into Elasticsearch
        var success = await elasticService.BulkIndexDocumentsAsync(
            documents,
            ElasticSearchIndexes.Problems
        );

        if (success)
        {
            logger.LogInformation("Seeded {Count} problems into Elasticsearch.", documents.Count);
        }
        else
        {
            logger.LogError("Failed to bulk index problems into Elasticsearch.");
        }
    }

    /// <summary>
    /// DTO used for seeding users into database
    /// </summary>
    public class SeedUser
    {
        public Guid Id { get; set; }

        public int RankName { get; set; }

        public string? ImagePath { get; set; }

        public int Rating { get; set; }

        public int? Gender { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public Guid? IdentityId { get; set; }
    }
}
