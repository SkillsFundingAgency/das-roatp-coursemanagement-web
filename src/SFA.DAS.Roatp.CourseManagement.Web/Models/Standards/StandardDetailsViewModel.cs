﻿using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using System.Collections.Generic;
using System.Linq;

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
        public string CourseDisplayName => $"{CourseName} (Level {Level})";

        public bool IsStandardRegulated => !string.IsNullOrEmpty(RegulatorName);

        public string StandardInfoUrl { get; set; }
        public string ContactUsPhoneNumber { get; set; }
        public string ContactUsEmail { get; set; }
        public string ContactUsPageUrl { get; set; }
        public List<ProviderCourseLocationViewModel> ProviderCourseLocations { get; set; }

        public List<ProviderCourseLocationViewModel> SubRegionCourseLocations { get; set; }
       
        public ProviderCourseLocationViewModel NationalCourseLocation { get; set; }

        public List<string> Regions
        {
            get
            {
                return SubRegionCourseLocations
                    .OrderBy(x => x.RegionName)
                    .Distinct().Select(region => region.RegionName).Distinct().ToList();
            }
        }


        public string LocationSummary
        {
            get
            {
                if (ProviderCourseLocations.Any())
                {
                    if (NationalCourseLocation != null)
                        return CourseDeliveryMessageFor.ProvidersAndNational;

                    if (SubRegionCourseLocations.Any())
                        return CourseDeliveryMessageFor.ProvidersAndSubregions;

                    return CourseDeliveryMessageFor.ProvidersOnly;
                }

                if (NationalCourseLocation != null)
                    return CourseDeliveryMessageFor.NationalOnly;

                if (SubRegionCourseLocations.Any())
                {
                    return CourseDeliveryMessageFor.SubregionsOnly;
                }

                return CourseDeliveryMessageFor.NoneSet;
            }
        }

        public string BackUrl { get; set; }
        public string EditContactDetailsUrl { get; set; }

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


    public static class CourseDeliveryMessageFor
    {
        public const string ProvidersOnly = "This standard is only delivered at your training locations.";
        public const string SubregionsOnly = "This standard is only delivered at an employer's address.";
        public const string NationalOnly = "This standard is only delivered at an employer's address anywhere in England.";
        public const string ProvidersAndNational = "This standard can be delivered at both training sites and employer addresses anywhere in England.";
        public const string ProvidersAndSubregions = "This standard can be delivered at both training sites and employer addresses within certain regions.";
        public const string NoneSet = "This standard has no training locations linked to it.";
    }
}
