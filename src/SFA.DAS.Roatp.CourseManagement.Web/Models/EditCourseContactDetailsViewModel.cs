using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models
{
    public class EditCourseContactDetailsViewModel : CourseContactDetailsSubmitModel, IBackLink
    {
        [FromRoute]
        public int LarsCode { get; set; }
        public int ProviderCourseId { get; set; }

        public static implicit operator EditCourseContactDetailsViewModel(GetStandardDetailsQueryResult standardDetails) =>
            new EditCourseContactDetailsViewModel
            {
                ContactUsEmail = standardDetails.ContactUsEmail,
                StandardInfoUrl = standardDetails.StandardInfoUrl,
                ContactUsPhoneNumber = standardDetails.ContactUsPhoneNumber
            };
    }
}
