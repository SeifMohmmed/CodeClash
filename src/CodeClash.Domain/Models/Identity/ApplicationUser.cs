using CodeClash.Domain.Premitives;
using Microsoft.AspNetCore.Identity;

namespace CodeClash.Domain.Models.Identity;
public class ApplicationUser : IdentityUser
{
    public short Rate { get; set; }
    public Status Status { get; set; } = Status.UnRanked;

}
