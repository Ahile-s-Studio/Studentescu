namespace Studentescu.ViewModels;

public class PaginatedList<T> : List<T>
{
    public PaginatedList(List<T> items, int count,
        int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages =
            (int)Math.Ceiling(count /
                              (double)pageSize);

        AddRange(items);
    }

    public int PageIndex { get; }
    public int TotalPages { get; }

    public bool HasPreviousPage => PageIndex > 1;

    public bool HasNextPage => PageIndex < TotalPages;

    public static PaginatedList<T> Create(
        IEnumerable<T> source, int count,
        int pageIndex, int pageSize)
    {
        var items = source
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PaginatedList<T>(items, count,
            pageIndex, pageSize);
    }
}