using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations
{
    public class ProviderLocationViewModel 
    {
        public int LocationId { get; set; }
        public int? RegionId { get; set; }
        public string LocationName { get; set; }
        public string Postcode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public static implicit operator ProviderLocationViewModel(ProviderLocation source)
        {
            return new ProviderLocationViewModel
            {
                LocationId = source.LocationId,
                RegionId = source.RegionId,
                LocationName = source.LocationName,
                Postcode = source.Postcode,
                Email = source.Email,
                Phone = source.Phone
            };
        }
    }
}
