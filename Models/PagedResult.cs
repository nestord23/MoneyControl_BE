namespace MoneyControl.Models;

public record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int Page, int PageSize, int TotalPages)
{
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public PagedResult<TTarget> Map<TTarget>(Func<T, TTarget> mapper)
    {
        return new PagedResult<TTarget>(
            Items.Select(mapper),
            TotalCount,
            Page,
            PageSize,
            TotalPages
        );
    }
}
