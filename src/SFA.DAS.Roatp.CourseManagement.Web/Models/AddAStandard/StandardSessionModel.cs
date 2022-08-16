﻿using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Collections.Generic;
using System.Linq;

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
        public IEnumerable<CourseLocationModel> ProviderLocations => CourseLocations.Where(l => l.LocationType == LocationType.Provider);
        public IEnumerable<IGrouping<string, CourseLocationModel>> RegionalLocations => CourseLocations.Where(l => l.LocationType == LocationType.Regional).GroupBy(l => l.RegionName).OrderBy(g => g.Key);
        public string LocationSummary => LocationSummaryCalculator.GetLocationSummary(HasNationalDeliveryOption.GetValueOrDefault(), ProviderLocations.Any(), RegionalLocations.Any());
        public string CancelLink { get; set; }
    }
}
