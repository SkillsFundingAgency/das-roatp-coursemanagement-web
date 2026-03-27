using System;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

public class ShortCourseTrainingVenuesSubmitModel : ShortCourseBaseViewModel
{
    public List<Guid> SelectedProviderLocationIds { get; set; } = new();
}
