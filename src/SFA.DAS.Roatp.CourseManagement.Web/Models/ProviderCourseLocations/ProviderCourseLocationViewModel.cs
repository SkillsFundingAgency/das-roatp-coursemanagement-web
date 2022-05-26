using SFA.DAS.Roatp.CourseManagement.Application.Constants;
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

        public CourseLocationDeliveryOption DeliveryOption()
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
            return CourseLocationDeliveryOption.NotSet;
        }
        
        // public string HasOffersPortableFlexiJob => OffersPortableFlexiJob != null && OffersPortableFlexiJob.Value ? "Yes" : "No";
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
