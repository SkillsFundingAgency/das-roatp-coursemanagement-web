using System;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourse
{
    public class ProviderCourseLocationCommandModel
    {
        public Guid ProviderLocationId { get; set; }
        public bool HasDayReleaseDeliveryOption { get; set; }
        public bool HasBlockReleaseDeliveryOption { get; set; }
    }
}
