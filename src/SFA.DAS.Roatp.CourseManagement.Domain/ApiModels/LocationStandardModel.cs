namespace SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

public class LocationStandardModel
{
    public string Title { get; set; }
    public int Level { get; set; }
    public int LarsCode { get; set; }
    public string StandardUrl { get; set; }

    public string CourseDisplayName => $"{Title} (level {Level})";

    public bool HasOtherVenues { get; set; }
}