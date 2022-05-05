using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models
{
    [ExcludeFromCodeCoverage]
    public class ReviewYourDetailsViewModel : ViewModelBase
    {
        public ReviewYourDetailsViewModel(HttpContext context) : base(context)
        {
        }

        public string DashboardUrl { get; set; }
    }
}
