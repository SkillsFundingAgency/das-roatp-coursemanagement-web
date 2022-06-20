using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails
{
    public class GetStandardDetailsQueryResult
    {
        public string CourseName { get; set; }
        public string Level { get; set; }
        public string IFateReferenceNumber { get; set; }
        public string Sector { get; set; }
        public int LarsCode { get; set; }
        public string RegulatorName { get; set; }
        public string Version { get; set; }
        public string StandardInfoUrl { get; set; }
        public string ContactUsPhoneNumber { get; set; }
        public string ContactUsEmail { get; set; }
        public string ContactUsPageUrl { get; set; }
        public bool? IsApprovedByRegulator { get; set; }
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

        public static implicit operator GetStandardDetailsQueryResult(StandardDetails v)
        {
            return new GetStandardDetailsQueryResult
            {
                ContactUsEmail = v.ContactUsEmail,
                ContactUsPageUrl = v.ContactUsPageUrl,
                ContactUsPhoneNumber = v.ContactUsPhoneNumber,
                CourseName = v.CourseName,
                IFateReferenceNumber = v.IFateReferenceNumber,
                LarsCode = v.LarsCode,
                Level = v.Level,
                ProviderCourseLocations = v.ProviderCourseLocations,
                RegulatorName = v.RegulatorName,
                Sector = v.Sector,
                StandardInfoUrl = v.StandardInfoUrl,
                Version = v.Version,
                IsApprovedByRegulator = v.IsApprovedByRegulator
            };
        }
    }
}