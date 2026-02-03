using Humanizer;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

public class ShortCourseBaseViewModel
{
    public CourseType? CourseType { get; set; }
    public string CourseTypeLower => CourseType?.Humanize(LetterCasing.LowerCase);
    public string CourseTypeLowerPlural => CourseType?.Humanize(LetterCasing.LowerCase).Pluralize();
}
