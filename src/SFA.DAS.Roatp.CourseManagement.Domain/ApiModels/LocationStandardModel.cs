namespace SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

public class LocationStandardModel
{
    public string Title { get; set; }
    public int Level { get; set; }
    public string LarsCode { get; set; }
    public string StandardUrl { get; set; }
    public string CourseDisplayName { get; set; }
    public bool HasOtherVenues { get; set; }
}