using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails
{
    public class GetProviderCourseDetailsQueryResult
    {
        public string CourseName { get; set; }
        public int Level { get; set; }
        public string IFateReferenceNumber { get; set; }
        public string Sector { get; set; }
        public string LarsCode { get; set; }
        public string RegulatorName { get; set; }
        public ApprenticeshipType ApprenticeshipType { get; set; }
        public string StandardInfoUrl { get; set; }
        public string ContactUsPhoneNumber { get; set; }
        public string ContactUsEmail { get; set; }
        public bool? IsApprovedByRegulator { get; set; }
        public bool IsRegulatedForProvider { get; set; }
        public bool HasLocations { get; set; }
        public bool HasOnlineDeliveryOption { get; set; }
        public CourseType CourseType { get; set; }
        public int Duration { get; set; }
        public DurationUnits DurationUnits { get; set; }
        public List<ProviderCourseLocation> ProviderCourseLocations { get; set; } = new List<ProviderCourseLocation>();

        public bool HasProviderLocation => ProviderCourseLocations.Any(l => l.LocationType == LocationType.Provider);
        public bool HasNationalLocation => ProviderCourseLocations.Any(l => l.LocationType == LocationType.National);
        public bool HasRegionalLocation => ProviderCourseLocations.Any(l => l.LocationType == LocationType.Regional);

        public LocationOption LocationOption
        {
            get
            {
                if (!ProviderCourseLocations.Any())
                    return LocationOption.None;
                if (HasProviderLocation && !HasNationalLocation && !HasRegionalLocation)
                    return LocationOption.ProviderLocation;
                else if (!HasProviderLocation && (HasNationalLocation || HasRegionalLocation))
                    return LocationOption.EmployerLocation;
                else
                    return LocationOption.Both;
            }
        }

        public static implicit operator GetProviderCourseDetailsQueryResult(StandardDetails v)
        {
            return new GetProviderCourseDetailsQueryResult
            {
                ContactUsEmail = v.ContactUsEmail,
                ContactUsPhoneNumber = v.ContactUsPhoneNumber,
                CourseName = v.CourseName,
                IFateReferenceNumber = v.IFateReferenceNumber,
                LarsCode = v.LarsCode,
                Level = v.Level,
                ProviderCourseLocations = v.ProviderCourseLocations,
                RegulatorName = v.RegulatorName,
                Sector = v.Sector,
                StandardInfoUrl = v.StandardInfoUrl,
                ApprenticeshipType = v.ApprenticeshipType,
                IsApprovedByRegulator = v.IsApprovedByRegulator,
                IsRegulatedForProvider = v.IsRegulatedForProvider,
                HasLocations = v.HasLocations,
                HasOnlineDeliveryOption = v.HasOnlineDeliveryOption,
                Duration = v.Duration,
                DurationUnits = v.DurationUnits
            };
        }
    }
}