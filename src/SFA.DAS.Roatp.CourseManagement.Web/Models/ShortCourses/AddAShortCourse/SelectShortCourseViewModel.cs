using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class SelectShortCourseViewModel : SelectShortCourseSubmitModel, IBackLink
{
    public IEnumerable<SelectListItem> ShortCourses { get; set; }
}

public class SelectShortCourseSubmitModel : ShortCourseBaseViewModel
{
    public string SelectedLarsCode { get; set; }
}