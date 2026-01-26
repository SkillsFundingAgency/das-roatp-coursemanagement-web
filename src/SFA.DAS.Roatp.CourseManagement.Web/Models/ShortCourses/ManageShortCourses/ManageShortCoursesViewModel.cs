using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

public class ManageShortCoursesViewModel : IBackLink
{
    public List<string> ShortCourses { get; set; } = new List<string>();
    public string AddAShortCourseLink { get; set; }
    public CourseType CourseType { get; set; }
}
