using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using System;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations
{
    public class ProviderLocationViewModel : ProviderLocationDetailsSubmitModel
    {
        public Guid NavigationId { get; set; }
        public int? RegionId { get; set; }
        public string VenueNameUrl { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string UpdateContactDetailsUrl { get; set; }
        public string BackUrl { get; set; }
        public string CancelUrl { get; set; }

        public static implicit operator ProviderLocationViewModel(ProviderLocation source)
        {
            return new ProviderLocationViewModel
            {
                NavigationId = source.NavigationId,
                RegionId = source.RegionId,
                LocationName = source.LocationName,
                AddressLine1 = source.AddressLine1,
                AddressLine2 = source.AddressLine2,
                Town = source.Town,
                County = source.County,
                Postcode = source.Postcode,
                EmailAddress = source.Email,
                Website = source.Website,
                PhoneNumber = source.Phone,
            };
        }
    }
}
