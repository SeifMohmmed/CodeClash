using CodeClash.Domain.Premitives;

namespace CodeClash.Application.Helpers;
public static class ElasticHelper
{
    static public string GetSortField(
        SortBy sortBy)
    {
        return sortBy switch
        {
            SortBy.Difficulty => "difficulty",
            SortBy.AcceptanceRate => "acceptanceRate",
            _ => "name.keyword"
        };
    }
}
