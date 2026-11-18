using SFA.DAS.Roatp.CourseManagement.Domain.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models;

public class EditLocationOptionsSessionModel
{
    public LocationOption ExistingLocationOption { get; set; }
    public LocationOption UpdatedLocationOption { get; set; }
    public bool HasExistingNationalOption { get; set; }
    public bool HasLocationOptionChanged => ExistingLocationOption != UpdatedLocationOption;
}
