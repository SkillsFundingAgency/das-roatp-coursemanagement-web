using System;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

public static class CommonHelpers
{
    public static DateTime GetDate(int year, int month, int day = 1) => new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Unspecified);
}
