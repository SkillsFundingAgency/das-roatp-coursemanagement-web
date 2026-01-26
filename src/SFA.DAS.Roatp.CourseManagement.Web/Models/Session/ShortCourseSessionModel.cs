using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.Constants;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Session;

public class ShortCourseSessionModel
{
    public string LarsCode { get; set; }
    public ShortCourseInformationViewModel ShortCourseInformation { get; set; } = new ShortCourseInformationViewModel();
    public ProviderContactModel LatestProviderContactModel { get; set; }
    public bool? IsUsingSavedContactDetails { get; set; }
    public ContactInformationSessionModel ContactInformation { get; set; } = new ContactInformationSessionModel();
    public List<ShortCourseLocationOption> ShortCourseLocations { get; set; } = new List<ShortCourseLocationOption>();
    public bool HasOnlineDeliveryOption { get; set; }
}
