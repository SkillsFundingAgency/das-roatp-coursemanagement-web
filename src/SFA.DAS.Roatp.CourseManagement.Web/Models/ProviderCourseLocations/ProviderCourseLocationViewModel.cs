using Microsoft.AspNetCore.Mvc;
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
        public bool? HasDayReleaseDeliveryOption { get; set; }
        public bool? HasBlockReleaseDeliveryOption { get; set; }

        public string RegionName { get; set; }

        public string DeliveryOption()
        {
            if ((HasDayReleaseDeliveryOption.HasValue && HasDayReleaseDeliveryOption.Value) &&
                (HasBlockReleaseDeliveryOption.HasValue && HasBlockReleaseDeliveryOption.Value))
            {
                return CourseLocationDeliveryOption.DayAndBlockRelease;
            }
            if (HasDayReleaseDeliveryOption.HasValue && HasDayReleaseDeliveryOption.Value)
            {
                return CourseLocationDeliveryOption.DayRelease;
            }
            if (HasBlockReleaseDeliveryOption.HasValue && HasBlockReleaseDeliveryOption.Value)
            {
                return CourseLocationDeliveryOption.BlockRelease;
            }
            return string.Empty;
        }
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
                HasDayReleaseDeliveryOption = providerCourseLocation.HasDayReleaseDeliveryOption,
                HasBlockReleaseDeliveryOption = providerCourseLocation.HasBlockReleaseDeliveryOption,
                RegionName = providerCourseLocation.RegionName
            };
        }
    }

    public static class CourseLocationDeliveryOption
    {
        public const string DayAndBlockRelease = "Day and block release";
        public const string DayRelease = "Day release";
        public const string BlockRelease = "Block release";
    }
}