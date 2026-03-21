using System.Linq.Expressions;
using CodeClash.Application.Authentication;
using CodeClash.Domain.Models.Identity;

namespace CodeClash.Application.Mapping;
public static class UserMappings
{
    public static Expression<Func<User, UserDto>> ProjectToDto()
    {
        return u => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            CreatedAtUtc = u.CreatedAtUtc,
            UpdatedAtUtc = u.UpdatedAtUtc,

            ImagePath = u.ImagePath,
            Rating = u.Rating,
            RankName = u.RankName.ToString(),
        };
    }
}
