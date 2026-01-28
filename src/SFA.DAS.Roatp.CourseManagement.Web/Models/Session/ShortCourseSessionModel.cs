using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAShortCourse;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Session;

public class ShortCourseSessionModel
{
    public string LarsCode { get; set; }
    public ShortCourseInformationViewModel ShortCourseInformation { get; set; } = new ShortCourseInformationViewModel();
    public ProviderContactModel LatestProviderContactModel { get; set; }
    public bool? IsUsingSavedContactDetails { get; set; }
    public ContactInformationSessionModel ContactInformation { get; set; } = new ContactInformationSessionModel();
}
