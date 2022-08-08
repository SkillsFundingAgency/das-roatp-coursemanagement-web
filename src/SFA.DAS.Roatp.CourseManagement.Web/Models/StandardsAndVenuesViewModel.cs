using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models
{
    [ExcludeFromCodeCoverage]
    public class StandardsAndVenuesViewModel
    {
        public string VenuesUrl { get; set; }
        public string StandardsUrl { get; set; }
        public string ProviderDescriptionUrl { get; set; }
        public string BackUrl { get; set; }
    }
}
