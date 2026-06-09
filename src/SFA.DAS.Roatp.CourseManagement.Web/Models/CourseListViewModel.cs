using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models;

public record CourseListViewModel(IEnumerable<CourseList> Courses);

public record CourseList(string CourseName);