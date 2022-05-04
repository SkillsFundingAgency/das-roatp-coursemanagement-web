using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class StandardListViewModel : BaseViewModel
    {
        public List<StandardViewModel> Standards { get; set; } = new List<StandardViewModel>();
    }
}
