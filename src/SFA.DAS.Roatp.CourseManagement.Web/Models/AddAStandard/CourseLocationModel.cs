using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourse;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using System;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard
{
    public class CourseLocationModel
    {
        public LocationType LocationType { get; set; }
        public Guid? ProviderLocationId { get; set; }
        public string LocationName { get; set; }
        public DeliveryMethodModel DeliveryMethod { get; set; }
        public int? SubregionId { get; set; }
        public string RegionName { get; set; }
        public string SubregionName { get; set; }

        public static implicit operator ProviderCourseLocationCommandModel(CourseLocationModel source) 
            => new ProviderCourseLocationCommandModel
            {
                ProviderLocationId = source.ProviderLocationId.GetValueOrDefault(),
                HasBlockReleaseDeliveryOption = source.DeliveryMethod.HasDayReleaseDeliveryOption.GetValueOrDefault(),
                HasDayReleaseDeliveryOption = source.DeliveryMethod.HasDayReleaseDeliveryOption.GetValueOrDefault()
            };
    }
}
