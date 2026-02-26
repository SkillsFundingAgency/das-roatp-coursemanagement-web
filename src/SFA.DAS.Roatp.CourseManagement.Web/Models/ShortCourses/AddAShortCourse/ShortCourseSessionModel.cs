using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourse;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class ShortCourseSessionModel
{
    public string LarsCode { get; set; }
    public ShortCourseInformationViewModel ShortCourseInformation { get; set; } = new ShortCourseInformationViewModel();
    public ProviderContactModel SavedProviderContactModel { get; set; }
    public bool? IsUsingSavedContactDetails { get; set; }
    public ContactInformationModel ContactInformation { get; set; } = new ContactInformationModel();
    public List<ShortCourseLocationOption> LocationOptions { get; set; } = new List<ShortCourseLocationOption>();
    public bool HasOnlineDeliveryOption { get; set; }
    public List<ProviderLocation> ProviderLocations { get; set; } = new List<ProviderLocation>();
    public List<TrainingVenueModel> TrainingVenues { get; set; } = new List<TrainingVenueModel>();
    public bool LocationsAvailable { get; set; }
    public bool? HasNationalDeliveryOption { get; set; }
    public List<TrainingRegionModel> TrainingRegions { get; set; } = new List<TrainingRegionModel>();
    public bool HasSeenSummaryPage { get; set; }
    public void ResetModel()
    {
        TrainingVenues = new List<TrainingVenueModel>();
        TrainingRegions = new List<TrainingRegionModel>();
        HasOnlineDeliveryOption = false;
        HasNationalDeliveryOption = null;
    }

    public static implicit operator AddProviderCourseCommand(ShortCourseSessionModel source) => new()
    {
        LarsCode = source.LarsCode,
        IsApprovedByRegulator = source.ShortCourseInformation.IsRegulatedForProvider ? true : null,
        StandardInfoUrl = source.ContactInformation.StandardInfoUrl,
        ContactUsEmail = source.ContactInformation.ContactUsEmail,
        ContactUsPhoneNumber = source.ContactInformation.ContactUsPhoneNumber,
        HasNationalDeliveryOption = source.HasNationalDeliveryOption.GetValueOrDefault(),
        HasOnlineDeliveryOption = source.HasOnlineDeliveryOption,
        SubregionIds = source.TrainingRegions.Select(l => l.SubregionId.Value).ToList(),
        ProviderLocations = source.TrainingVenues.Select(l => (ProviderCourseLocationCommandModel)l).ToList()
    };
}
