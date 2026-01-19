using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ManageShortCourses;

public class ManageShortCoursesViewModel : IBackLink
{
    public List<string> ShortCourses { get; set; } = new List<string>();
    public string AddAShortCourseLink { get; set; }
    public string CourseTypeDescription { get; set; }
    public string CourseTypeHeading { get; set; }
}
