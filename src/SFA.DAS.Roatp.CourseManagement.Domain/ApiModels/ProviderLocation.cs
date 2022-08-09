using System;

namespace SFA.DAS.Roatp.CourseManagement.Domain.ApiModels
{
    public class ProviderLocation
    {
        public Guid NavigationId { get; set; }
        public int? RegionId { get; set; }
        public string LocationName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Phone { get; set; }
        public LocationType LocationType { get; set; }
    }
}
