using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class StandardListViewModel 
    {
        public List<StandardViewModel> Standards { get; set; } = new List<StandardViewModel>();
        public string BackLink { get; set; }
        public string AddAStandardLink { get; set; }
    }
}
