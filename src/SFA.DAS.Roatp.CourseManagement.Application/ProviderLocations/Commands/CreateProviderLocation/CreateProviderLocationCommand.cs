using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.CreateProviderLocation
{
    public class CreateProviderLocationCommand : IRequest<Unit>
    {
        public int Ukprn { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public string LocationName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; } = string.Empty;
        public string Town { get; set; }
        public string Postcode { get; set; }
        public string County { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
