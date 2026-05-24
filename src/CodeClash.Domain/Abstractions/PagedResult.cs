namespace CodeClash.Domain.Abstractions;
public sealed class PagedResult<T>
{
    public IEnumerable<T> Items { get; }
    public int TotalPages { get; }

    public PagedResult(
        IEnumerable<T> items,
        int totalPages)
    {
        Items = items;
        TotalPages = totalPages;
    }
}
