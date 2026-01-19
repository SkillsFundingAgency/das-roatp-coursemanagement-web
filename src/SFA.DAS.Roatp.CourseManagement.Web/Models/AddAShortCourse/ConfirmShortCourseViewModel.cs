using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAShortCourse;

public class ConfirmShortCourseViewModel : ConfirmShortCourseSubmitModel, IBackLink
{
    public ShortCourseInformationViewModel ShortCourseInformation { get; set; }
}

public class ConfirmShortCourseSubmitModel
{
    public bool? IsCorrectShortCourse { get; set; }
    public CourseType CourseType { get; set; }
}