namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAnApprenticeshipUnit;

public class ConfirmApprenticeshipUnitViewModel : ConfirmApprenticeshipUnitSubmitModel, IBackLink
{
    public ShortCourseInformationViewModel ShortCourseInformation { get; set; }
}

public class ConfirmApprenticeshipUnitSubmitModel
{
    public bool? IsCorrectShortCourse { get; set; }
}