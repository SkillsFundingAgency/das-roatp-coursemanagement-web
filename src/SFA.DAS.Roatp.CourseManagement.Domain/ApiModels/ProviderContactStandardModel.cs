using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;

namespace SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

public class ProviderContactStandardModel
{
    public int ProviderCourseId { get; set; }
    public string CourseName { get; set; }
    public int Level { get; set; }
    public bool IsSelected { get; set; }
    public CourseType CourseType { get; set; }

    public static implicit operator ProviderContactStandardModel(Standard s)
    {
        return new ProviderContactStandardModel
        {
            ProviderCourseId = s.ProviderCourseId,
            CourseName = s.CourseName,
            Level = s.Level,
            CourseType = s.CourseType
        };
    }
}