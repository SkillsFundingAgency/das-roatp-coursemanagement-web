using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Helpers;

public static class ShortCourseLocationDisplayHelper
{
    public const string ProviderLocationText = "At your training venue";
    public const string EmployerLocationText = "At employer’s location";
    public const string OnlineText = "Online";
    public static string MapLocationOptionsDisplayText(ShortCourseLocationOption locationOption) => locationOption switch
    {
        ShortCourseLocationOption.ProviderLocation => ProviderLocationText,
        ShortCourseLocationOption.EmployerLocation => EmployerLocationText,
        ShortCourseLocationOption.Online => OnlineText,
        _ => locationOption.ToString()
    };
}