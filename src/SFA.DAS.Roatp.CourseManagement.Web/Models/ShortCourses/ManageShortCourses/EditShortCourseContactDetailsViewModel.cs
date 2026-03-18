using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

public class EditShortCourseContactDetailsViewModel : ShortCourseContactDetailsViewModel, IBackLink
{
    public static implicit operator EditShortCourseContactDetailsViewModel(GetStandardDetailsQueryResult standardDetails) =>
        new EditShortCourseContactDetailsViewModel
        {
            ContactUsEmail = standardDetails.ContactUsEmail,
            StandardInfoUrl = standardDetails.StandardInfoUrl,
            ContactUsPhoneNumber = standardDetails.ContactUsPhoneNumber
        };
}