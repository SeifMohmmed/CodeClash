using CodeClash.Domain.Abstractions;
using CodeClash.Domain.Models.Blogs;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Models.Submits;
using CodeClash.Domain.Models.TestCases;
using CodeClash.Domain.Models.Topics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CodeClash.Infrastructure;
public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    public DbSet<Contest> Contests { get; set; }
    public DbSet<UserContest> Registers { get; set; }
    public DbSet<Problem> Problems { get; set; }
    public DbSet<ProblemImage> ProblemImages { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<BlogImage> TutorialImages { get; set; }
    public DbSet<Testcase> Testcases { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<ProblemTopic> ProblemTopics { get; set; }
    public DbSet<Submit> Submits { get; set; }
    public DbSet<User> Users { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Automatically apply IEntityTypeConfiguration<T>
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Schemas.Application);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        return await Database.BeginTransactionAsync(cancellationToken);
    }
}
