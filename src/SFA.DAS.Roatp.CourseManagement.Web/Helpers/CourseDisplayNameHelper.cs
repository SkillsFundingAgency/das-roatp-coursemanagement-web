namespace SFA.DAS.Roatp.CourseManagement.Web.Helpers;

public static class CourseDisplayNameHelper
{
    public static string BuildCourseDisplayName(string courseName, int courseLevel)
    {
        return $"{courseName} (level {courseLevel})";
    }
}
