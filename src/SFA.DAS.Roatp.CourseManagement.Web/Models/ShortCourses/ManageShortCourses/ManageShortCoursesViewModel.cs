using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

public class ManageShortCoursesViewModel : ShortCourseBaseViewModel
{
    public List<ShortCourseViewModel> ShortCourses { get; set; } = new List<ShortCourseViewModel>();
    public string AddAShortCourseLink { get; set; }
    public bool ShowShortCourseHeading { get; set; }
    public bool ShowDeleteShortCourseNotificationBanner { get; set; }
    public BannerViewModel Banner { get; set; } = new BannerViewModel();
    public int TotalCourseCount { get; set; }
    public bool HasCourses { get; set; }
    public bool HasNoCourses { get; set; }
    public BreadcrumbsViewModel Breadcrumbs { get; set; }

    public void SetCourseValues()
    {
        TotalCourseCount = ShortCourses.Count;

        HasCourses = TotalCourseCount > 0;

        HasNoCourses = TotalCourseCount == 0;
    }
}
