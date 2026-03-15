using CodeClash.Domain.Models.Blogs;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Models.Submits;
using CodeClash.Domain.Models.TestCases;
using CodeClash.Domain.Models.Topics;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.Infrastructure.Context;
public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public new DbSet<ApplicationUser> Users { get; set; }
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

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Automatically apply IEntityTypeConfiguration<T>
        builder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(builder);
    }

}
