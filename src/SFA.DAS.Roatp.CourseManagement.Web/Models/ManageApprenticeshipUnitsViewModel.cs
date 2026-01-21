using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models;

public class ManageApprenticeshipUnitsViewModel : IBackLink
{
    public List<string> ApprenticeshipUnits { get; set; } = new List<string>();
    public string AddAnApprenticeshipUnitLink { get; set; }
}
