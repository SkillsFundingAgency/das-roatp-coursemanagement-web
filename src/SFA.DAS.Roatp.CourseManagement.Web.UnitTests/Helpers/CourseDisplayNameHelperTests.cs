using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Helpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Helpers;

public class CourseDisplayNameHelperTests
{
    [Test]

    public void WhenInvokingHelper_ThenReturnCourseDisplayCorrectly()
    {
        // Arrange
        string courseTitle = "Test";
        int courseLevel = 1;

        // Act
        var result = CourseDisplayNameHelper.BuildCourseDisplayName(courseTitle, courseLevel);

        // Assert
        Assert.That(result, Is.EqualTo("Test (level 1)"));
    }
}