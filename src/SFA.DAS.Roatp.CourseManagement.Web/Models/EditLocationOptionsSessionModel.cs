using SFA.DAS.Roatp.CourseManagement.Domain.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models;

public class EditLocationOptionsSessionModel
{
    public LocationSelectionsModel Existing { get; set; } = new();
    public LocationSelectionsModel Updated { get; set; } = new();
    public bool HasExistingNationalOption { get; set; }
}

public class LocationSelectionsModel
{
    public LocationOption? LocationOption { get; set; }
    public bool? HasNationalOption { get; set; }
}
