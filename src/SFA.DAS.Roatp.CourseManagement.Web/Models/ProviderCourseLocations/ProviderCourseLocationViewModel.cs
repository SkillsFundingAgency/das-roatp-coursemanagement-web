﻿using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using System;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations
{

    public class ProviderCourseLocationViewModel
    {
        [FromRoute]
        public int LarsCode { get; set; }
        public Guid Id { get; set; }
        public string LocationName { get; set; }
        public LocationType LocationType { get; set; }
        public string RegionName { get; set; }
        public string SubregionName { get; set; }

        public DeliveryMethodModel DeliveryMethod { get; set; } = new DeliveryMethodModel();
        public string RemoveUrl { get; set; }
        public string BackLink { get; set; }
        public string CancelLink { get; set; }
        public static implicit operator ProviderCourseLocationViewModel(ProviderCourseLocation providerCourseLocation)
        {
            return new ProviderCourseLocationViewModel
            {
                Id = providerCourseLocation.Id,
                LocationName = providerCourseLocation.LocationName,
                LocationType = providerCourseLocation.LocationType,
                DeliveryMethod = new DeliveryMethodModel 
                { 
                    HasDayReleaseDeliveryOption = providerCourseLocation.HasDayReleaseDeliveryOption,
                    HasBlockReleaseDeliveryOption = providerCourseLocation.HasBlockReleaseDeliveryOption
                },
                RegionName = providerCourseLocation.RegionName,
                SubregionName = providerCourseLocation.SubregionName
            };
        }
    }
}