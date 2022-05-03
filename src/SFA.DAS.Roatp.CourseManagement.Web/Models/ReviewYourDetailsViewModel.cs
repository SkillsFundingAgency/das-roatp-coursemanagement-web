using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models
{
    [ExcludeFromCodeCoverage]
    public class ReviewYourDetailsViewModel 
    {
        public string DashboardUrl { get; set; }
        public Dictionary<string, string> RouteDictionary { get; set; }
    }
}
