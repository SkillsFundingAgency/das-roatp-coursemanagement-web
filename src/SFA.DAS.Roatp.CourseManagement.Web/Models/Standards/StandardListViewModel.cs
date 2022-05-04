using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class StandardListViewModel : ViewModelBase
    {
        public StandardListViewModel(HttpContext context) : base(context)
        {
        }
        public List<StandardViewModel> Standards { get; set; } = new List<StandardViewModel>();
    }
}
