using CodeClash.Application.Abstractions.Identity;
using CodeClash.Application.Abstractions.RankUp;
using CodeClash.Domain.Premitives;
using Microsoft.EntityFrameworkCore;

namespace CodeClash.Infrastructure.Implementation;

public sealed class RankUpService(
    IAppDbContext db) : IRankUpService
{
    public async Task LevelUpUserRank(
        string userId,
        ContestPoints points,
        CancellationToken cancellationToken)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.IdentityId == userId, cancellationToken);

        if (user is null)
        {
            return;
        }

        user.Rating += (short)points;
        user.RankName = GetStatus(user.Rating);

        await db.SaveChangesAsync(cancellationToken);
    }

    private static UserStatus GetStatus(short rating) => rating switch
    {
        0 => UserStatus.UnRanked,
        < 400 => UserStatus.Newbie,
        < 800 => UserStatus.Pupil,
        < 1200 => UserStatus.Specialist,
        < 1600 => UserStatus.Expert,
        < 2000 => UserStatus.Candidate_Master,
        < 2400 => UserStatus.Master,
        _ => UserStatus.International_Master
    };
}
