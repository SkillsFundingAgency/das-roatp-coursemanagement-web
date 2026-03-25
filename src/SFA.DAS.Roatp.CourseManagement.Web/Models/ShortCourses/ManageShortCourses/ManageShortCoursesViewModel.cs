using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

public class ManageShortCoursesViewModel : ShortCourseBaseViewModel, IBackLink
{
    public List<ShortCourseViewModel> ShortCourses { get; set; } = new List<ShortCourseViewModel>();
    public string AddAShortCourseLink { get; set; }
    public bool ShowShortCourseHeading { get; set; }
    public bool ShowDeleteShortCourseNotificationBanner { get; set; }
    public BannerViewModel Banner { get; set; } = new BannerViewModel();
}
