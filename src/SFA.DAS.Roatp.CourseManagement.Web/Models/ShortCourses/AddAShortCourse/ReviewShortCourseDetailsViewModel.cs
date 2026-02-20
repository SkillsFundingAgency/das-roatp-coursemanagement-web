using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class ReviewShortCourseDetailsViewModel : ShortCourseBaseViewModel
{
    public ShortCourseInformationViewModel ShortCourseInformation { get; set; }
    public string ContactUsEmail { get; set; }
    public string ContactUsPhoneNumber { get; set; }
    public string StandardInfoUrl { get; set; }
    public List<ShortCourseLocationOption> LocationOptions { get; set; }
    public List<string> DeliveryLocations { get; set; }
    public List<string> TrainingVenues { get; set; }
    public string HasNationalDeliveryOption { get; set; }
    public List<string> TrainingRegions { get; set; }
    public bool DeliversAtEmployerLocation { get; set; }
    public string ContactDetailsChangeLink { get; set; } = "#";
    public string LocationOptionsChangeLink { get; set; } = "#";
    public string TrainingVenuesChangeLink { get; set; } = "#";
    public string NationalProviderChangeLink { get; set; } = "#";
    public string TrainingRegionsChangeLink { get; set; } = "#";
    public string CancelLink { get; set; } = "#";

    public static implicit operator ReviewShortCourseDetailsViewModel(ShortCourseSessionModel sessionModel) => new()
    {
        ShortCourseInformation = sessionModel.ShortCourseInformation,
        ContactUsEmail = sessionModel.ContactInformation.ContactUsEmail,
        ContactUsPhoneNumber = sessionModel.ContactInformation.ContactUsPhoneNumber,
        StandardInfoUrl = sessionModel.ContactInformation.StandardInfoUrl,
        LocationOptions = sessionModel.LocationOptions,
        DeliveryLocations = sessionModel.LocationOptions.Select(MapLocationOptionsDisplayText).ToList(),
        TrainingVenues = sessionModel.TrainingVenues.Select(x => x.LocationName).ToList(),
        HasNationalDeliveryOption = sessionModel.HasNationalDeliveryOption switch { true => "Yes", false => "No", _ => null },
        TrainingRegions = sessionModel.TrainingRegions.Select(x => x.SubregionName).ToList(),
        DeliversAtEmployerLocation = sessionModel.LocationOptions.Contains(ShortCourseLocationOption.EmployerLocation)
    };

    private static string MapLocationOptionsDisplayText(ShortCourseLocationOption locationOption) => locationOption switch
    {
        ShortCourseLocationOption.ProviderLocation => "At your training venue",
        ShortCourseLocationOption.EmployerLocation => "At employer’s location",
        ShortCourseLocationOption.Online => "Online",
        _ => locationOption.ToString()
    };
}

