using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models;

public record CourseLinksViewModel(IEnumerable<CourseLink> Courses);

public record CourseLink(string Name, string Url);
