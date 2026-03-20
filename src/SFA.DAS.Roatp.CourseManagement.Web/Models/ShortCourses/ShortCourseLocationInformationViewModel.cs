using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

public class ShortCourseLocationInformationViewModel : ShortCourseBaseViewModel
{
    public List<ShortCourseLocationOption> LocationOptions { get; set; }
    public List<string> DeliveryLocations { get; set; }
    public List<string> TrainingVenues { get; set; }
    public string HasNationalDeliveryOption { get; set; }
    public List<string> TrainingRegions { get; set; }
    public bool DeliversAtEmployerLocation { get; set; }
    public string LocationOptionsChangeLink { get; set; } = "#";
    public string TrainingVenuesChangeLink { get; set; } = "#";
    public string NationalProviderChangeLink { get; set; } = "#";
    public string TrainingRegionsChangeLink { get; set; } = "#";
    public bool ShowTrainingVenues { get; set; }
    public bool ShowTrainingRegions { get; set; }
}
