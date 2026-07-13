namespace DoctorsManagementSystem.Desktop.ViewModels.Pages;

public class PageButtonItem
{
    public int PageNumber { get; init; }
    public bool IsEllipsis { get; init; }
    public bool IsCurrentPage { get; init; }

    public static PageButtonItem Ellipsis() => new() { IsEllipsis = true };
    public static PageButtonItem Page(int pageNumber, bool isCurrent) =>
        new() { PageNumber = pageNumber, IsCurrentPage = isCurrent };
}