using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

public class DeleteShortCourseViewModel : ShortCourseBaseViewModel, IBackLink
{
    public ShortCourseInformationViewModel ShortCourseInformation { get; set; }
    public string BackToManageShortCoursesLink { get; set; } = "#";

    public static implicit operator DeleteShortCourseViewModel(GetStandardInformationQueryResult source)
        => new DeleteShortCourseViewModel
        {
            ShortCourseInformation = (ShortCourseInformationViewModel)source
        };
}