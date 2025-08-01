using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourse;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.Services;


namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard
{
    public class StandardSessionModel
    {
        public int LarsCode { get; set; }
        public bool IsConfirmed { get; set; }
        public StandardInformationViewModel StandardInformation { get; set; } = new StandardInformationViewModel();
        public StandardContactInformationViewModel ContactInformation { get; set; } = new StandardContactInformationViewModel();
        public LocationOption LocationOption { get; set; }
        public bool? HasNationalDeliveryOption { get; set; }
        public List<CourseLocationModel> CourseLocations { get; set; } = new List<CourseLocationModel>();

        [JsonIgnore]
        public IEnumerable<CourseLocationModel> ProviderLocations => CourseLocations.Where(l => l.LocationType == LocationType.Provider);

        [JsonIgnore]
        public IEnumerable<IGrouping<string, CourseLocationModel>> RegionalLocations => CourseLocations.Where(l => l.LocationType == LocationType.Regional).GroupBy(l => l.RegionName).OrderBy(g => g.Key);

        [JsonIgnore]
        public string LocationSummary => LocationSummaryCalculator.GetLocationSummary(HasNationalDeliveryOption.GetValueOrDefault(), ProviderLocations.Any(), RegionalLocations.Any());

        [JsonIgnore]
        public string CancelLink { get; set; }

        public static implicit operator AddProviderCourseCommand(StandardSessionModel source)
            => new AddProviderCourseCommand
            {
                LarsCode = source.LarsCode,
                IsApprovedByRegulator = source.StandardInformation.IsRegulatedForProvider,
                StandardInfoUrl = source.ContactInformation.StandardInfoUrl,
                ContactUsEmail = source.ContactInformation.ContactUsEmail,
                ContactUsPageUrl = source.ContactInformation.ContactUsPageUrl,
                ContactUsPhoneNumber = source.ContactInformation.ContactUsPhoneNumber,
                HasNationalDeliveryOption = source.HasNationalDeliveryOption.GetValueOrDefault(),
                SubregionIds = source.CourseLocations.Where(l => l.LocationType == LocationType.Regional).Select(l => l.SubregionId.Value).ToList(),
                ProviderLocations = source.CourseLocations.Where(l => l.LocationType == LocationType.Provider).Select(l => (ProviderCourseLocationCommandModel)l).ToList()
            };
    }
}
