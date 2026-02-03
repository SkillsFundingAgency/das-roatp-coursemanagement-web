using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class ShortCourseSessionModel
{
    public string LarsCode { get; set; }
    public CourseType CourseType { get; set; }
    public ShortCourseInformationViewModel ShortCourseInformation { get; set; } = new ShortCourseInformationViewModel();
    public ProviderContactModel SavedProviderContactModel { get; set; }
    public bool? IsUsingSavedContactDetails { get; set; }
    public ContactInformationModel ContactInformation { get; set; } = new ContactInformationModel();
    public List<ShortCourseLocationOption> LocationOptions { get; set; } = new List<ShortCourseLocationOption>();
    public bool HasOnlineDeliveryOption { get; set; }
    public List<TrainingVenueModel> TrainingVenues { get; set; } = new List<TrainingVenueModel>();
}
