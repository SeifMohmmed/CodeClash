using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Abstractions.RankUp;

public interface IRankUpService
{
    Task LevelUpUserRank(string userId,
        ContestPoints points,
        CancellationToken cancellationToken);
}
