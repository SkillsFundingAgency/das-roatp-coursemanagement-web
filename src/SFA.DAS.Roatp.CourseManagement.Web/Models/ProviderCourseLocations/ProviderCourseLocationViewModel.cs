using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations
{

    public class ProviderCourseLocationViewModel
    {
        public string LocationName { get; set; }
        public LocationType LocationType { get; set; }
        public bool? HasDayReleaseDeliveryOption { get; set; }
        public bool? HasBlockReleaseDeliveryOption { get; set; }
        public bool? OffersPortableFlexiJob { get; set; }
        public string RegionName { get; set; }
        public string DeliveryOption()
        {
            if (HasBlockReleaseDeliveryOption != null && HasDayReleaseDeliveryOption != null && (HasDayReleaseDeliveryOption.Value) && HasBlockReleaseDeliveryOption.Value)
            {
                return "Day & block release";
            }
            if (HasDayReleaseDeliveryOption != null && HasDayReleaseDeliveryOption.Value)
            {
                return "Day release";
            }
            if (HasBlockReleaseDeliveryOption != null && HasBlockReleaseDeliveryOption.Value)
            {
                return "Block release";
            }
            return string.Empty;
        }
        public string HasOffersPortableFlexiJob => OffersPortableFlexiJob != null && OffersPortableFlexiJob.Value ? "Yes" : "No";
        public static implicit operator ProviderCourseLocationViewModel(ProviderCourseLocation providerCourseLocation)
        {
            return new ProviderCourseLocationViewModel
            {
                LocationName = providerCourseLocation.LocationName,
                LocationType = providerCourseLocation.LocationType,
                HasDayReleaseDeliveryOption = providerCourseLocation.HasDayReleaseDeliveryOption,
                HasBlockReleaseDeliveryOption = providerCourseLocation.HasBlockReleaseDeliveryOption,
                OffersPortableFlexiJob = providerCourseLocation.OffersPortableFlexiJob,
                RegionName = providerCourseLocation.RegionName
            };
        }
    }
}
