using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

public class DeleteShortCourseViewModel : ShortCourseBaseViewModel, IBackLink
{
    public ShortCourseInformationViewModel ShortCourseInformation { get; set; }
    public string BackToManageShortCoursesLink { get; set; } = "#";

    public static implicit operator DeleteShortCourseViewModel(GetProviderCourseDetailsQueryResult source)
        => new DeleteShortCourseViewModel
        {
            ShortCourseInformation = (ShortCourseInformationViewModel)source
        };
}