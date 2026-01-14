namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAnApprenticeshipUnit;

public class ConfirmApprenticeshipUnitViewModel : ConfirmApprenticeshipUnitSubmitModel, IBackLink
{
    public CourseInformation CourseInformation { get; set; }
}

public class ConfirmApprenticeshipUnitSubmitModel
{
    public bool? IsCorrectCourse { get; set; }
}