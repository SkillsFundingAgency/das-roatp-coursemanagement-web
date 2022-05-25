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
        private const string DayAndBlockReleaseDeliveryOption = "Day & block release";
        private const string DayReleaseDeliveryOption = "Day release";
        private const string BlockReleaseDeliveryOption = "Block release";
        private const string Yes = "Yes";
        private const string No = "No";
        public string DeliveryOption()
        {
            if ((HasDayReleaseDeliveryOption.HasValue && HasDayReleaseDeliveryOption.Value) && 
                (HasBlockReleaseDeliveryOption.HasValue && HasBlockReleaseDeliveryOption.Value))
            {
                return DayAndBlockReleaseDeliveryOption;
            }
            if (HasDayReleaseDeliveryOption.HasValue && HasDayReleaseDeliveryOption.Value)
            {
                return DayReleaseDeliveryOption;
            }
            if (HasBlockReleaseDeliveryOption.HasValue && HasBlockReleaseDeliveryOption.Value)
            {
                return BlockReleaseDeliveryOption;
            }
            return string.Empty;
        }
        public string HasOffersPortableFlexiJob => OffersPortableFlexiJob.HasValue && OffersPortableFlexiJob.Value ? Yes : No;
        public static implicit operator ProviderCourseLocationViewModel(ProviderCourseLocation providerCourseLocation)
        {
            return new ProviderCourseLocationViewModel
            {
                LocationName = providerCourseLocation.LocationName,
                LocationType = providerCourseLocation.LocationType,
                HasDayReleaseDeliveryOption = providerCourseLocation.HasDayReleaseDeliveryOption,
                HasBlockReleaseDeliveryOption = providerCourseLocation.HasBlockReleaseDeliveryOption,
                OffersPortableFlexiJob = providerCourseLocation.OffersPortableFlexiJob,
            };
        }
    }
}
