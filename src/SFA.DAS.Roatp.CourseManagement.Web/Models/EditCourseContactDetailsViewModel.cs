using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models
{
    public class EditCourseContactDetailsViewModel : EditCourseContactDetailsSubmitModel
    {
        public string BackLink { get; set; }
        public string CancelLink { get; set; }
        public int ProviderCourseId { get; set; }

        public static implicit operator EditCourseContactDetailsViewModel(GetStandardDetailsQueryResult standardDetails) =>
            new EditCourseContactDetailsViewModel
            {
                ContactUsEmail = standardDetails.ContactUsEmail,
                ContactUsPageUrl = standardDetails.ContactUsPageUrl,
                StandardInfoUrl = standardDetails.StandardInfoUrl,
                ContactUsPhoneNumber = standardDetails.ContactUsPhoneNumber
            };
    }
}
