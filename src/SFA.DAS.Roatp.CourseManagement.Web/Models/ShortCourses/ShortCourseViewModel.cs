using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

public class ShortCourseViewModel
{
    public int ProviderCourseId { get; set; }
    public string CourseName { get; set; }
    public int Level { get; set; }
    public string CourseDisplayName { get; set; }
    public string LarsCode { get; set; }
    public string StandardUrl { get; set; }
    public bool HasLocation { get; set; }
    public bool StandardRequiresMoreInfo => SetMissingInfo();

    public static implicit operator ShortCourseViewModel(Standard source)
    {
        return new ShortCourseViewModel
        {
            ProviderCourseId = source.ProviderCourseId,
            CourseName = source.CourseName,
            Level = source.Level,
            CourseDisplayName = source.DisplayName,
            LarsCode = source.LarsCode,
            HasLocation = source.HasLocations
        };
    }

    private bool SetMissingInfo()
    {
        return (!HasLocation);
    }
}
