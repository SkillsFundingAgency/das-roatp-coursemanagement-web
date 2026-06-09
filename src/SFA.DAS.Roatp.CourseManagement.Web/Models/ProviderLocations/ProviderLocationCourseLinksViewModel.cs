using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;

public record ProviderLocationCourseLinksViewModel(IEnumerable<ProviderLocationCourseLink> Courses);

public record ProviderLocationCourseLink(string CourseName, string Url);