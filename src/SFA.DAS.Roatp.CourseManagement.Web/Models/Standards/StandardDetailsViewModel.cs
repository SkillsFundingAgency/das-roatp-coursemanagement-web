﻿using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using SFA.DAS.Roatp.CourseManagement.Application.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class StandardDetailsViewModel
    {
        public int LarsCode { get; set; }
        public string CourseName { get; set; }
        public string Level { get; set; } 
        public string IFateReferenceNumber { get; set; }
        public string Sector { get; set; }
        public string RegulatorName { get; set; }
        public string Version { get; set; }
        public string CourseDisplayName
        {
            get => $"{CourseName} (Level {Level})";
            set => throw new System.NotImplementedException();
        }

        public bool IsStandardRegulated => !string.IsNullOrEmpty(RegulatorName);

        public string StandardInfoUrl { get; set; }
        public string ContactUsPhoneNumber { get; set; }
        public string ContactUsEmail { get; set; }
        public string ContactUsPageUrl { get; set; }
        public List<ProviderCourseLocationViewModel> ProviderCourseLocations { get; set; }

        public List<ProviderCourseLocationViewModel> SubRegionCourseLocations { get; set; }
       
        public ProviderCourseLocationViewModel NationalCourseLocation { get; set; }
        public List<string> Regions  =>
            SubRegionCourseLocations
                .OrderBy(x => x.RegionName)
                .Distinct().Select(region => region.RegionName).Distinct().ToList();
        public WhereIsStandardDelivered LocationSummary()
        {
            if (ProviderCourseLocations.Any())
            {
                if (NationalCourseLocation != null)
                    return WhereIsStandardDelivered.ProvidersAndNational;

                if (SubRegionCourseLocations.Any())
                    return WhereIsStandardDelivered.ProviersAndSubregions;

                return WhereIsStandardDelivered.ProvidersOnly;
            }

            return NationalCourseLocation != null ? WhereIsStandardDelivered.NationalOnly : WhereIsStandardDelivered.SubregionsOnly;
        }

        public string BackUrl { get; set; }

        public static implicit operator StandardDetailsViewModel(StandardDetails standardDetails)
        {
            return new StandardDetailsViewModel
            {
                CourseName = standardDetails.CourseName,
                Level = standardDetails.Level,
                IFateReferenceNumber = standardDetails.IFateReferenceNumber,
                LarsCode = standardDetails.LarsCode,
                RegulatorName = standardDetails.RegulatorName,
                Sector = standardDetails.Sector,
                Version = standardDetails.Version,
                StandardInfoUrl = standardDetails.StandardInfoUrl,
                ContactUsPhoneNumber = standardDetails.ContactUsPhoneNumber,
                ContactUsEmail = standardDetails.ContactUsEmail,
                ContactUsPageUrl = standardDetails.ContactUsPageUrl,
                ProviderCourseLocations = standardDetails.ProviderCourseLocations.Where(a => a.LocationType == LocationType.Provider).Select(x => (ProviderCourseLocationViewModel)x).ToList(),
                SubRegionCourseLocations = standardDetails.ProviderCourseLocations.Where(a => a.LocationType == LocationType.Regional).Select(x => (ProviderCourseLocationViewModel)x).ToList(),
                NationalCourseLocation = standardDetails.ProviderCourseLocations.Where(a => a.LocationType == LocationType.National).Select(x => (ProviderCourseLocationViewModel)x).FirstOrDefault()
            };
        }
    }
}
