using MediatR;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.CreateProviderLocation
{
    public class AddProviderCourseLocationCommand : IRequest<Unit>
    {
        public int Ukprn { get; set; }
        public int LarsCode { get; }
        public string UserId { get; set; }
        public string LocationName { get; set; }
        public bool? HasDayReleaseDeliveryOption { get; set; }
        public bool? HasBlockReleaseDeliveryOption { get; set; }
    }
}
