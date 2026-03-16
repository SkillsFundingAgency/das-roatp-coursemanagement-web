using System.Linq;
using SFA.DAS.Roatp.CourseManagement.Web.Helpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class ReviewShortCourseDetailsViewModel : ShortCourseBaseViewModel
{
    public ShortCourseInformationViewModel ShortCourseInformation { get; set; }
    public ShortCourseContactInformationViewModel ContactInformation { get; set; }
    public ShortCourseLocationInformationViewModel LocationInformation { get; set; }
    public string CancelLink { get; set; } = "#";

    public static implicit operator ReviewShortCourseDetailsViewModel(ShortCourseSessionModel sessionModel) => new()
    {
        ShortCourseInformation = sessionModel.ShortCourseInformation,
        ContactInformation = new ShortCourseContactInformationViewModel()
        {
            ContactUsEmail = sessionModel.ContactInformation.ContactUsEmail,
            ContactUsPhoneNumber = sessionModel.ContactInformation.ContactUsPhoneNumber,
            StandardInfoUrl = sessionModel.ContactInformation.StandardInfoUrl
        },
        LocationInformation = new ShortCourseLocationInformationViewModel()
        {
            LocationOptions = sessionModel.LocationOptions,
            DeliveryLocations = sessionModel.LocationOptions.Select(ShortCourseLocationDisplayHelper.MapLocationOptionsDisplayText).ToList(),
            TrainingVenues = sessionModel.TrainingVenues.Select(x => x.LocationName).ToList(),
            HasNationalDeliveryOption = sessionModel.HasNationalDeliveryOption switch { true => "Yes", false => "No", _ => null },
            TrainingRegions = sessionModel.TrainingRegions.Select(x => x.SubregionName).OrderBy(x => x).ToList(),
            DeliversAtEmployerLocation = sessionModel.LocationOptions.Contains(ShortCourseLocationOption.EmployerLocation),
            ShowTrainingVenues = sessionModel.TrainingVenues.Count != 0,
            ShowTrainingRegions = sessionModel.TrainingRegions.Count != 0
        }
    };
}

