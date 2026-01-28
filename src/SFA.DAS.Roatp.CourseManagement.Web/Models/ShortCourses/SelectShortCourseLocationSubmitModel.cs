using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.Constants;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

public class SelectShortCourseLocationSubmitModel
{
    public List<ShortCourseLocationOption> ShortCourseLocations { get; set; } = new List<ShortCourseLocationOption>();
}
