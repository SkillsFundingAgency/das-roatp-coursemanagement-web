using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models;

public class ShortCourseShutterPagesViewModel : ShortCourseTrainingVenuesSubmitModel
{
    public BreadcrumbsViewModel Breadcrumbs { get; set; } = new BreadcrumbsViewModel();
    public string Link { get; set; } = string.Empty;
    public string RegulatedBy { get; set; } = string.Empty;
}
