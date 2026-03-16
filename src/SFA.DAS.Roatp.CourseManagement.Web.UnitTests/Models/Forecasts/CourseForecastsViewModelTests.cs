using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Forecasts;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Forecasts;

public class CourseForecastsViewModelTests
{
    [TestCase("1st Mar 2023", true)]
    [TestCase("", false)]
    [TestCase(null, false)]
    public void ShowLastUpdatedDate_HasCorrectValue(string value, bool expected)
    {
        CourseForecastsViewModel sut = new() { LastUpdatedDate = value };

        sut.ShowLastUpdatedDate.Should().Be(expected);
    }
}
