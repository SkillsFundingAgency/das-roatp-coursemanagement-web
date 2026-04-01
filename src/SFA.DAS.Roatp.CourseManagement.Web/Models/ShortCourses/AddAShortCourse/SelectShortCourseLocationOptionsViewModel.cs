using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class SelectShortCourseLocationOptionsViewModel : SelectShortCourseLocationOptionsSubmitModel, IBackLink
{
    public List<ShortCourseLocationOptionModel> LocationOptions { get; set; } = new List<ShortCourseLocationOptionModel>();
    public string SubmitButtonText { get; set; }
    public string Route { get; set; }
    public bool IsAddJourney { get; set; }
}
