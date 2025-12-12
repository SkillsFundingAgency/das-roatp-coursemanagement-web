using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;

public class ProviderLocationStandardModel
{
    public string Title { get; set; }
    public int Level { get; set; }
    public string LarsCode { get; set; }
    public string CourseDisplayName { get; set; }
    public string StandardUrl { get; set; }

    public static implicit operator ProviderLocationStandardModel(LocationStandardModel source)
    {
        return new ProviderLocationStandardModel
        {
            Title = source.Title,
            Level = source.Level,
            LarsCode = source.LarsCode,
            CourseDisplayName = $"{source.Title} (level {source.Level})"
        };
    }
}