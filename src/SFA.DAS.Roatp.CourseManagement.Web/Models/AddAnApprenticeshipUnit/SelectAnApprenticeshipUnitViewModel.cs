using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddAnApprenticeshipUnit;

public class SelectAnApprenticeshipUnitViewModel : SelectAnApprenticeshipUnitSubmitModel, IBackLink
{
    public IEnumerable<SelectListItem> ApprenticeshipUnit { get; set; }
}

public class SelectAnApprenticeshipUnitSubmitModel
{
    public string SelectedLarsCode { get; set; }

}