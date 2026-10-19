using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.Standards
{
    public class StandardListViewModel
    {
        public List<StandardViewModel> Standards { get; set; } = new List<StandardViewModel>();
        public string AddAStandardLink { get; set; }
        public bool ShowNotificationBannerDeleteStandard { get; set; }
        public bool ShowNotificationBannerAddStandard { get; set; }
        public int CoursesMissingInfoCount { get; set; }
        public bool ShowCourseMissingInfo { get; set; }
        public bool ShowSingleCourseMissingInfo { get; set; }
        public bool ShowListOfCoursesMissingInfo { get; set; }
        public int TotalCourseCount { get; set; }
        public bool HasCourses { get; set; }
        public bool HasNoCourses { get; set; }
        public BreadcrumbsViewModel Breadcrumbs { get; set; }

        public void SetCourseValues()
        {
            CoursesMissingInfoCount = Standards.Count(s => s.StandardRequiresMoreInfo);

            ShowCourseMissingInfo = CoursesMissingInfoCount > 0;

            ShowSingleCourseMissingInfo = CoursesMissingInfoCount == 1;

            ShowListOfCoursesMissingInfo = CoursesMissingInfoCount > 1;

            TotalCourseCount = Standards.Count;

            HasCourses = TotalCourseCount > 0;

            HasNoCourses = TotalCourseCount == 0;
        }
    }
}
