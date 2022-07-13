namespace SFA.DAS.Roatp.CourseManagement.Domain.ApiModels
{
    public class ProviderLocation
    {
        public int LocationId { get; set; }
        public int? RegionId { get; set; }
        public string LocationName { get; set; }
        public string Postcode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public LocationType LocationType { get; set; }
    }
}
