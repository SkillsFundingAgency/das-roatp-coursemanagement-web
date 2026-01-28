using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class SelectShortCourseViewModel : SelectShortCourseSubmitModel, IBackLink
{
    public IEnumerable<SelectListItem> ShortCourses { get; set; }
}

public class SelectShortCourseSubmitModel
{
    public string SelectedLarsCode { get; set; }
    public required CourseType CourseType { get; set; }

}