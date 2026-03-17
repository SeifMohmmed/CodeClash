using Bogus;
using CodeClash.Application.Abstractions.Data;
using Dapper;

namespace CodeClash.API.Extensions;

public static class SeedDataExtension
{
    /// <summary>
    /// Extension method used to seed the database with fake apartment data.
    /// It runs once at application startup to populate the database for testing/demo purposes.
    /// </summary>
    public static void SeedData(this IApplicationBuilder app)
    {
        // Create a new dependency injection scope
        using var scope = app.ApplicationServices.CreateScope();

        // Resolve the SQL connection factory from DI container
        var sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();

        // Create a database connection
        using var connection = sqlConnectionFactory.CreateConnection();

        // Bogus library used to generate fake data
        var faker = new Faker();

        //--------------------------------
        // Generate Users
        //--------------------------------

        var users = new Faker<SeedUser>()
            .RuleFor(u => u.Id, _ => Guid.NewGuid().ToString())
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.UserName, f => f.Internet.UserName())
            .RuleFor(u => u.NormalizedUserName, (f, u) => u.UserName.ToUpper())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email.ToUpper())
            .RuleFor(u => u.Rating, f => f.Random.Int(800, 2400))
            .RuleFor(u => u.RankName, f => f.Random.Int(0, 5))
            .RuleFor(u => u.SecurityStamp, _ => Guid.NewGuid().ToString())
            .RuleFor(u => u.ConcurrencyStamp, _ => Guid.NewGuid().ToString())
            .RuleFor(u => u.EmailConfirmed, _ => true)
            .RuleFor(u => u.PasswordHash, _ => "")
            .RuleFor(u => u.PhoneNumberConfirmed, _ => false)
            .RuleFor(u => u.TwoFactorEnabled, _ => false)
            .RuleFor(u => u.LockoutEnabled, _ => false)
            .RuleFor(u => u.AccessFailedCount, _ => 0);

        var userList = users.Generate(20);

        const string userSql = """
                INSERT INTO users
                (id, rank_name, image_path, rating, gender, name,
                 user_name, normalized_user_name, email, normalized_email,
                 email_confirmed, password_hash, security_stamp, concurrency_stamp,
                 phone_number, phone_number_confirmed, two_factor_enabled,
                 lockout_end, lockout_enabled, access_failed_count)
                VALUES
                (@Id, @RankName, @ImagePath, @Rating, @Gender, @Name,
                 @UserName, @NormalizedUserName, @Email, @NormalizedEmail,
                 @EmailConfirmed, @PasswordHash, @SecurityStamp, @ConcurrencyStamp,
                 @PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled,
                 @LockoutEnd, @LockoutEnabled, @AccessFailedCount);
            """;

        connection.Execute(userSql, userList);

        //--------------------------------
        // Get User Ids
        //--------------------------------

        var userIds = connection
            .Query<string>("SELECT id FROM \"users\"")
            .ToList();

        //--------------------------------
        // Generate Contests
        //--------------------------------

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

        const string contestSql = """
            INSERT INTO contests
            (id, name, setter_id, start_date, end_date, blog_id)
            VALUES
            (@Id, @Name, @SetterId, @StartDate, @EndDate, @BlogId);
        """;

        connection.Execute(contestSql, contests);

        //--------------------------------
        // Get Contest Ids
        //--------------------------------

        var contestIds = connection
            .Query<Guid>("SELECT id FROM contests")
            .ToList();


        //--------------------------------
        // Generate Problems
        //--------------------------------

        // List that will contain all generated apartment objects
        List<object> problems = new();

        // Generate 100 fake apartment records
        for (int i = 0; i < 100; i++)
        {
            problems.Add(new
            {
                //Problem Data
                Id = Guid.NewGuid(),

                Name = faker.Lorem.Sentence(3),

                SetterId = faker.PickRandom(userIds),// change to existing user

                ContestId = faker.PickRandom(contestIds), // or use real contest id

                Difficulty = faker.Random.Int(0, 2), // Easy Medium Hard enum

                ContestPoints = faker.Random.Int(100, 1000),

                Description = faker.Lorem.Paragraphs(3),

                RunTimeLimit = faker.Random.Decimal(1, 3),

                MemoryLimit = faker.Random.Decimal(128, 512),

                BlogId = (Guid?)null
            });
        }

        // SQL query used to insert Problems records into PostgreSQL
        const string problemSql = """
            INSERT INTO problems
            (id, name, setter_id, contest_id, difficulty,
             contest_points, description, run_time_limit, memory_limit, blog_id)
            VALUES
            (@Id, @Name, @SetterId, @ContestId, @Difficulty,
             @ContestPoints, @Description, @RunTimeLimit, @MemoryLimit, @BlogId);
        """;

        // Execute the insert query using Dapper
        // This will insert all problems in a batch operation
        connection.Execute(problemSql, problems);
    }
}

public class SeedUser
{
    public string Id { get; set; }

    public int RankName { get; set; }

    public string? ImagePath { get; set; }

    public int Rating { get; set; }

    public int? Gender { get; set; }

    public string Name { get; set; }

    public string UserName { get; set; }

    public string NormalizedUserName { get; set; }

    public string Email { get; set; }

    public string NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string PasswordHash { get; set; }

    public string SecurityStamp { get; set; }

    public string ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTime? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }
}
