using Humanizer;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

public class ShortCourseBaseViewModel
{
    public LearningType? LearningType { get; set; }
    public string LearningTypeLower => LearningType?.Humanize(LetterCasing.LowerCase);
    public string LearningTypeLowerPlural => LearningType?.Humanize(LetterCasing.LowerCase).Pluralize();
    public string LearningTypeHumanize => LearningType?.Humanize();
}
