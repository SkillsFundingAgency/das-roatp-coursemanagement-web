using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class SelectShortCourseTrainingVenueViewModel : SelectShortCourseTrainingVenueSubmitModel, IBackLink
{
    public CourseType CourseType { get; set; }
}