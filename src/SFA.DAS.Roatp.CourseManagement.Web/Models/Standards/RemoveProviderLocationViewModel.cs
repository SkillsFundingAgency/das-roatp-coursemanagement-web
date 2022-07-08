using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class RemoveProviderLocationViewModel
    {
        [FromRoute]
        public int LarsCode { get; set; }
        public int Id { get; set; }
        public string BackLink { get; set; }
        public string CancelLink { get; set; }
        public static implicit operator RemoveProviderLocationViewModel(ProviderCourseLocation source)
        {
            return new RemoveProviderLocationViewModel
            {
                Id = source.Id
            };
        }
    }
}
