using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddShortCourses;

public class SelectShortCourseViewModel : SelectShortCourseSubmitModel, IBackLink
{
    public IEnumerable<SelectListItem> ShortCourses { get; set; }
}

public class SelectShortCourseSubmitModel
{
    public string SelectedLarsCode { get; set; }
    public string CourseTypeDescription { get; set; }

}