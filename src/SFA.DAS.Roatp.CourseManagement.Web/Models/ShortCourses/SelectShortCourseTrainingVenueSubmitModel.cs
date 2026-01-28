using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using System;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

public class SelectShortCourseTrainingVenueSubmitModel
{
    public List<Guid> SelectedProviderLocationIds { get; set; } = new();
    public List<ProviderLocation> ProviderLocations { get; set; } = new List<ProviderLocation>();
}
