using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

public class ManageShortCoursesViewModel : ShortCourseBaseViewModel, IBackLink
{
    public List<string> ShortCourses { get; set; } = new List<string>();
    public string AddAShortCourseLink { get; set; }
}
