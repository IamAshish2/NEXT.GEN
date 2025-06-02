namespace NEXT.GEN.Models.UserModel.pagination;

public class PaginatedList<T>
{
    public List<T> Items { get; set; }
    public int PageIndex { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }

    public PaginatedList(List<T> items, int pageIndex, int totalPages, int totalItems)
    {
        Items = items;
        PageIndex = pageIndex;
        TotalPages = totalPages;
        TotalItems = totalItems;
    }
}