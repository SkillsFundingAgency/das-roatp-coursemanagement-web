using System;

namespace SFA.DAS.Roatp.CourseManagement.Domain.ApiModels
{
    public class ProviderCourseLocation
    {
        public Guid Id { get; set; }
        public string LocationName { get; set; }
        public LocationType LocationType { get; set; }
        public bool? HasDayReleaseDeliveryOption { get; set; }
        public bool? HasBlockReleaseDeliveryOption { get; set; }
        public string RegionName { get; set; }
    }
}