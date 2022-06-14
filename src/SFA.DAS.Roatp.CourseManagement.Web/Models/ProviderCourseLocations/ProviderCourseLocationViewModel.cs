using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations
{

    public class ProviderCourseLocationViewModel
    {
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

        public static implicit operator ProviderCourseLocationViewModel(ProviderCourseLocation providerCourseLocation)
        {
            return new ProviderCourseLocationViewModel
            {
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