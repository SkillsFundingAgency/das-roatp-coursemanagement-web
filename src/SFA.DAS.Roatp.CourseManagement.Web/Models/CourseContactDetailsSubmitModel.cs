using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateContactDetails;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models
{
    public class CourseContactDetailsSubmitModel
    {
        public string ContactUsEmail { get; set; }
        public string ContactUsPhoneNumber { get; set; }

        public string StandardInfoUrl { get; set; }

        public static implicit operator UpdateProviderCourseContactDetailsCommand(CourseContactDetailsSubmitModel model) =>
            new UpdateProviderCourseContactDetailsCommand
            {
                ContactUsEmail = model.ContactUsEmail,
                StandardInfoUrl = model.StandardInfoUrl,
                ContactUsPhoneNumber = model.ContactUsPhoneNumber
            };

    }
}
