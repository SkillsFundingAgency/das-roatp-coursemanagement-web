using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

public class SelectShortCourseLocationOptionsSubmitModel
{
    public List<ShortCourseLocationOption> SelectedLocationOptions { get; set; } = new();
}
