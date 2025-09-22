namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard
{
    public class AddStandardContactDetailsViewModel : CourseContactDetailsSubmitModel, IBackLink
    {
        public bool ShowSavedContactDetailsText { get; set; }
        public string BackUrl { get; set; }
    }
}
