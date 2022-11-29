using MediatR;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourse
{
    public class AddProviderCourseCommand : IRequest<Unit>
    {
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public int Ukprn { get; set; }
        public int LarsCode { get; set; }
        public bool? IsApprovedByRegulator { get; set; }
        public string StandardInfoUrl { get; set; }
        public string ContactUsPhoneNumber { get; set; }
        public string ContactUsEmail { get; set; }
        public string ContactUsPageUrl { get; set; }
        public bool HasNationalDeliveryOption { get; set; }
        public List<ProviderCourseLocationCommandModel> ProviderLocations { get; set; }
        public List<int> SubregionIds { get; set; }
    }
}
