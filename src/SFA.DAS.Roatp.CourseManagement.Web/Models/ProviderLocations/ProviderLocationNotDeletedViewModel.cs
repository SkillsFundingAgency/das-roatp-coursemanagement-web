namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;

public class ProviderLocationNotDeletedViewModel : IBackLink
{
    public string LocationName { get; set; }
    public ProviderLocationCourseLinksViewModel StandardLinks { get; set; }
    public ProviderLocationCourseLinksViewModel ApprenticeshipUnitLinks { get; set; }
    public bool ShowStandards { get; set; }
    public bool ShowApprenticeshipUnits { get; set; }
}
