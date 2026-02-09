using Humanizer;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

public class ShortCourseBaseViewModel
{
    public ApprenticeshipType? ApprenticeshipType { get; set; }
    public string ApprenticeshipTypeLower => ApprenticeshipType?.Humanize(LetterCasing.LowerCase);
    public string ApprenticeshipTypeLowerPlural => ApprenticeshipType?.Humanize(LetterCasing.LowerCase).Pluralize();
}
