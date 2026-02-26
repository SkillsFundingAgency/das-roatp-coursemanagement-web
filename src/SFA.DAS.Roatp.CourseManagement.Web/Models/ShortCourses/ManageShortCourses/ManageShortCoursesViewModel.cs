using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

public class ManageShortCoursesViewModel : ShortCourseBaseViewModel, IBackLink
{
    public List<StandardViewModel> ShortCourses { get; set; } = new List<StandardViewModel>();
    public string AddAShortCourseLink { get; set; }
    public bool ShowShortCourseHeading { get; set; }
}
