using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Helpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

public class ManageShortCourseDetailsViewModel : ShortCourseBaseViewModel, IBackLink
{
    public ShortCourseInformationViewModel ShortCourseInformation { get; set; }
    public ShortCourseContactInformationViewModel ContactInformation { get; set; }
    public ShortCourseLocationInformationViewModel LocationInformation { get; set; }
    public string DeleteShortCourseLink { get; set; } = "#";
    public string BackToManageShortCoursesLink { get; set; } = "#";

    public static implicit operator ManageShortCourseDetailsViewModel(GetProviderCourseDetailsQueryResult source)
    {
        var locationOptions = new List<ShortCourseLocationOption>();
        if (source.HasProviderLocation)
            locationOptions.Add(ShortCourseLocationOption.ProviderLocation);
        if (source.HasRegionalLocation || source.HasNationalLocation)
            locationOptions.Add(ShortCourseLocationOption.EmployerLocation);
        if (source.HasOnlineDeliveryOption)
            locationOptions.Add(ShortCourseLocationOption.Online);

        return new ManageShortCourseDetailsViewModel
        {
            ShortCourseInformation = new ShortCourseInformationViewModel
            {
                LarsCode = source.LarsCode,
                CourseName = source.CourseName,
                Level = source.Level,
                IfateReferenceNumber = source.IFateReferenceNumber,
                Sector = source.Sector,
                RegulatorName = source.RegulatorName,
                ApprenticeshipType = source.ApprenticeshipType,
                IsRegulatedForProvider = source.IsRegulatedForProvider,
                Duration = source.Duration,
                DurationUnits = source.DurationUnits,
                CourseType = source.CourseType
            },
            ContactInformation = new ShortCourseContactInformationViewModel()
            {
                ContactUsEmail = source.ContactUsEmail,
                ContactUsPhoneNumber = source.ContactUsPhoneNumber,
                StandardInfoUrl = source.StandardInfoUrl
            },
            LocationInformation = new ShortCourseLocationInformationViewModel()
            {
                LocationOptions = locationOptions,
                DeliveryLocations = locationOptions.Select(ShortCourseLocationDisplayHelper.MapLocationOptionsDisplayText).ToList(),
                TrainingVenues = source.ProviderCourseLocations.Where(x => x.LocationType == LocationType.Provider).Select(x => x.LocationName).OrderBy(x => x).ToList(),
                HasNationalDeliveryOption = source.HasNationalLocation switch { true => "Yes", false => "No" },
                TrainingRegions = source.ProviderCourseLocations.Where(x => x.LocationType == LocationType.Regional).Select(x => x.SubregionName).OrderBy(x => x).ToList(),
                DeliversAtEmployerLocation = locationOptions.Contains(ShortCourseLocationOption.EmployerLocation),
                ShowTrainingVenues = source.ProviderCourseLocations.Any(x => x.LocationType == LocationType.Provider),
                ShowTrainingRegions = source.ProviderCourseLocations.Any(x => x.LocationType == LocationType.Regional),
            }
        };
    }
}
