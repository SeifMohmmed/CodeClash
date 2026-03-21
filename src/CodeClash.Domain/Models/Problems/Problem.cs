using System.ComponentModel.DataAnnotations.Schema;
using CodeClash.Domain.Models.Blogs;
using CodeClash.Domain.Models.Contests;
using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Models.Submits;
using CodeClash.Domain.Models.TestCases;
using CodeClash.Domain.Premitives;

namespace CodeClash.Domain.Models.Problems;
public sealed class Problem : Entity
{
    public string Name { get; set; }

    public string SetterId { get; set; }

    public Guid ContestId { get; set; }

    public Difficulty Difficulty { get; set; }

    public ContestPoints ContestPoints { get; set; }

    public string Description { get; set; }

    public decimal RunTimeLimit { get; set; }

    public decimal MemoryLimit { get; set; }


    [ForeignKey(nameof(ContestId))]
    public Contest Contest { get; set; }

    public Guid? BlogId { get; set; }

    [ForeignKey(nameof(BlogId))]
    public Blog Blog { get; set; }

    [ForeignKey(nameof(SetterId))]
    public User ProblemSetter { get; set; }


    public ICollection<ProblemImage> Images { get; set; }

    public ICollection<Testcase> Testcases { get; set; }

    public ICollection<ProblemTopic> ProblemTopics { get; set; }

    public ICollection<Submit> Submissions { get; set; }

}
