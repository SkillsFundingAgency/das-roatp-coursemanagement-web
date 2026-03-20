using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Helpers;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Helpers;
public class ShortCourseLocationDisplayHelperTests
{
    [Test]
    public void MapLocationOptionsDisplayText_ProviderLocation_ReturnsTrainingVenueText()
    {
        // Arrange
        var location = ShortCourseLocationOption.ProviderLocation;

        // Act
        var result = ShortCourseLocationDisplayHelper.MapLocationOptionsDisplayText(location);

        // Assert
        result.Should().Be("At your training venue");
    }

    [Test]
    public void MapLocationOptionsDisplayText_EmployerLocation_ReturnsEmployerLocationText()
    {
        // Arrange
        var location = ShortCourseLocationOption.EmployerLocation;

        // Act
        var result = ShortCourseLocationDisplayHelper.MapLocationOptionsDisplayText(location);

        // Assert
        result.Should().Be("At employer’s location");
    }

    [Test]
    public void MapLocationOptionsDisplayText_Online_ReturnsOnlineText()
    {
        // Arrange
        var location = ShortCourseLocationOption.Online;

        // Act
        var result = ShortCourseLocationDisplayHelper.MapLocationOptionsDisplayText(location);

        // Assert
        result.Should().Be("Online");
    }

    [Test]
    public void MapLocationOptionsDisplayText_UnknownEnum_ReturnsEnumToString()
    {
        // Arrange
        var location = (ShortCourseLocationOption)10;

        // Act
        var result = ShortCourseLocationDisplayHelper.MapLocationOptionsDisplayText(location);

        // Assert
        result.Should().Be(location.ToString());
    }
}
