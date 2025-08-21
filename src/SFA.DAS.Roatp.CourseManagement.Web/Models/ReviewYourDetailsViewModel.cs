using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models
{
    [ExcludeFromCodeCoverage]
    public class ReviewYourDetailsViewModel
    {
        public string StandardsUrl { get; set; }
        public string ProviderLocationsUrl { get; set; }
        public string BackUrl { get; set; }
        public string ProviderDescriptionUrl { get; set; }
        public string ProviderContactUrl { get; set; }
    }
}
