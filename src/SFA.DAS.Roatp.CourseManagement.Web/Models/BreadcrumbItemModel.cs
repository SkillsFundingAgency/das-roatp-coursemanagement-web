namespace SFA.DAS.Roatp.CourseManagement.Web.Models;

public class BreadcrumbItemModel
{
    public string Description { get; }
    public string Url { get; }

    public BreadcrumbItemModel(string description, string url)
    {
        Description = description;
        Url = url;
    }
}
