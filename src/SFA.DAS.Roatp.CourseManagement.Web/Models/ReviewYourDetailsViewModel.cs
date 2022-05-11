using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models
{
    [ExcludeFromCodeCoverage]
    public class ReviewYourDetailsViewModel : ViewModelBase
    {
        public string StandardsUrl { get; set; }
        public string ProviderLocationsUrl { get; set; }
    }
}
