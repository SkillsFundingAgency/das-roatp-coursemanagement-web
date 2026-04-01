using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

public class ShortCourseContactDetailsViewModel : CourseContactDetailsSubmitModel, IBackLink
{
    public bool ShowSavedContactDetailsText { get; set; }
    public bool IsAddJourney { get; set; }
    public string SubmitButtonText { get; set; }
    public string Route { get; set; }

    public static implicit operator ShortCourseContactDetailsViewModel(GetProviderCourseDetailsQueryResult standardDetails) =>
    new ShortCourseContactDetailsViewModel
    {
        ContactUsEmail = standardDetails.ContactUsEmail,
        StandardInfoUrl = standardDetails.StandardInfoUrl,
        ContactUsPhoneNumber = standardDetails.ContactUsPhoneNumber
    };
}
