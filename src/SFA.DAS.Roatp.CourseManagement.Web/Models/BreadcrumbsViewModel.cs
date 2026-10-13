using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models;

public class BreadcrumbsViewModel
{
    private readonly List<BreadcrumbItemModel> _items = new();
    public IReadOnlyList<BreadcrumbItemModel> Items => _items;

    public void Add(string description, string url)
    {
        _items.Add(new BreadcrumbItemModel(description, url ?? string.Empty));
    }
}
