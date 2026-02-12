using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

public class TrainingRegionModel
{
    public int? SubregionId { get; set; }
    public string SubregionName { get; set; }

    public static implicit operator TrainingRegionModel(RegionModel source) => new()
    {
        SubregionId = source.Id,
        SubregionName = source.SubregionName
    };
}
