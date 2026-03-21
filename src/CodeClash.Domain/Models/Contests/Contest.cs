using System.ComponentModel.DataAnnotations.Schema;
using CodeClash.Domain.Models.Blogs;
using CodeClash.Domain.Models.Identity;
using CodeClash.Domain.Models.Problems;
using CodeClash.Domain.Models.Submits;
using CodeClash.Domain.Premitives;

namespace CodeClash.Domain.Models.Contests;
public sealed class Contest : Entity
{
    public string Name { get; set; }

    public string SetterId { get; set; }

    public TimeSpan Duration => EndDate.Subtract(StartDate);

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public ContestStatus ContestStatus
    {
        get
        {
            var now = DateTime.Now;

            if (now < StartDate)
            {
                return ContestStatus.Upcoming;
            }

            if (now > EndDate)
            {
                return ContestStatus.Ended;
            }

            return ContestStatus.Running;
        }
    }
    public Guid? BlogId { get; set; }

    [ForeignKey(nameof(BlogId))]
    public Blog Blog { get; set; }


    [ForeignKey(nameof(SetterId))]
    public User ProblemSetter { get; set; }

    public ICollection<UserContest> Registrations { get; set; }
    public ICollection<Problem> Problems { get; set; }
    public ICollection<Submit> Submissions { get; set; }
}
