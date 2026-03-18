using System.ComponentModel.DataAnnotations.Schema;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Premitives;

namespace CodeClash.Domain.Models.TestCases;
public sealed class Testcase : Entity
{
    public Guid ProblemId { get; set; }

    public string Input { get; set; }

    public string Output { get; set; }

    [ForeignKey(nameof(ProblemId))]
    [InverseProperty(nameof(Problem.Testcases))]
    public Problem Problem { get; set; }
}

