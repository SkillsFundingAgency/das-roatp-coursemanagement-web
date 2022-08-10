using MediatR;
using System;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourseLocation
{
    public class AddProviderCourseLocationCommand : IRequest<Unit>
    {
        public int Ukprn { get; set; }
        public int LarsCode { get; set; }
        public string UserId { get; set; }
        public string LocationName { get; set; }
        public Guid LocationNavigationId { get; set; }
        public bool? HasDayReleaseDeliveryOption { get; set; }
        public bool? HasBlockReleaseDeliveryOption { get; set; }
    }
}
