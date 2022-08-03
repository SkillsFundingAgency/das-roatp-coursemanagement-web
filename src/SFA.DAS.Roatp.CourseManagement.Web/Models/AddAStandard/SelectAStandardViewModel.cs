using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard
{
    public class SelectAStandardViewModel : SelectAStandardSubmitModel
    {
        public string CancelLink { get; set; }
        public IEnumerable<SelectListItem> Standards { get; set; }
    }

    public class SelectAStandardSubmitModel
    {
        public int SelectedLarsCode { get; set; }

    }
}
