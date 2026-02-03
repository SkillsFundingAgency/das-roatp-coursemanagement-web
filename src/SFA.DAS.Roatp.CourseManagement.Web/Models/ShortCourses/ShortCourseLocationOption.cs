using System.ComponentModel;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

public enum ShortCourseLocationOption
{
    [Description("At one of your training locations")]
    ProviderLocation,
    [Description("At an employer's location")]
    EmployerLocation,
    [Description("Online")]
    Online
}
