namespace SFA.DAS.Roatp.CourseManagement.Domain.ApiModels
{
    public class AddressItem
    {
        public string Uprn { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
