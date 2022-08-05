using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateContactDetails;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models
{
    public class EditCourseContactDetailsSubmitModel
    {
        [FromRoute]
        public int LarsCode { get; set; }
        public string ContactUsEmail { get; set; }
        public string ContactUsPhoneNumber { get; set; }
        public string ContactUsPageUrl { get; set; }
        public string StandardInfoUrl { get; set; }

        public static implicit operator UpdateProviderCourseContactDetailsCommand(EditCourseContactDetailsSubmitModel model) =>
            new UpdateProviderCourseContactDetailsCommand
            {
                LarsCode = model.LarsCode,
                ContactUsEmail = model.ContactUsEmail,
                ContactUsPageUrl = model.ContactUsPageUrl,
                StandardInfoUrl = model.StandardInfoUrl,
                ContactUsPhoneNumber = model.ContactUsPhoneNumber
            };
    }
}
