using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Forecasts;

public class ForecastCoursesViewModel : ShortCourseBaseViewModel, IBackLink
{
    public CourseLinksViewModel CourseLinks { get; set; }
}
