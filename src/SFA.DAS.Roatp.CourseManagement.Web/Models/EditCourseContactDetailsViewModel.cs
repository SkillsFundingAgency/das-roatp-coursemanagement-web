using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateContactDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models
{
    public class EditCourseContactDetailsViewModel
    {
        [FromRoute]
        public int LarsCode { get; set; }
        public string BackLink { get; set; }
        public string CancelLink { get; set; }
        public int ProviderCourseId { get; set; }
        public string ContactUsEmail { get; set; }
        public string ContactUsPhoneNumber { get; set; }
        public string ContactUsPageUrl { get; set; }
        public string StandardInfoUrl { get; set; }

        public static implicit operator UpdateProviderCourseContactDetailsCommand(EditCourseContactDetailsViewModel model) => 
            new UpdateProviderCourseContactDetailsCommand
            {
                LarsCode = model.LarsCode,
                ContactUsEmail = model.ContactUsEmail,
                ContactUsPageUrl = model.ContactUsPageUrl,
                StandardInfoUrl = model.StandardInfoUrl,
                ContactUsPhoneNumber = model.ContactUsPhoneNumber
            };

        public static implicit operator EditCourseContactDetailsViewModel(StandardDetails standardDetails) =>
            new EditCourseContactDetailsViewModel
            {
                ContactUsEmail = standardDetails.ContactUsEmail,
                ContactUsPageUrl = standardDetails.ContactUsPageUrl,
                StandardInfoUrl = standardDetails.StandardInfoUrl,
                ContactUsPhoneNumber = standardDetails.ContactUsPhoneNumber
            };
    }
}
