using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard
{
    public class SelectAStandardViewModel : SelectAStandardSubmitModel, IBackLink
    {
        public IEnumerable<SelectListItem> Standards { get; set; }
    }

    public class SelectAStandardSubmitModel
    {
        public string SelectedLarsCode { get; set; }

    }
}
