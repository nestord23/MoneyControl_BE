namespace MoneyControl.DTOs;

public record PagedResponse<T>(IEnumerable<T> Items, int TotalCount, int Page, int PageSize, int TotalPages)
{
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
