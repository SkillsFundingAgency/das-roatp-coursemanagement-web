namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

public class ConfirmNationalDeliveryViewModel : ConfirmNationalDeliverySubmitModel, IBackLink
{
    public string SubmitButtonText { get; set; }
    public string Route { get; set; }
    public bool IsAddJourney { get; set; }
}
