using System;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderCourseForecasts.Commands.UpsertProviderCourseForecasts;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Forecasts;
using static SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers.CommonHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Forecasts;

public class ForecastSubmitModelTests
{
    [TestCase(1, "1st")]
    [TestCase(2, "2nd")]
    [TestCase(3, "3rd")]
    [TestCase(4, "4th")]
    public void GetDateRange_Returns_Correctly_Formatted_String(int dayOfMonth, string expected)
    {
        // Arrange
        DateTime startDate = GetDate(2024, 1, dayOfMonth);
        DateTime endDate = GetDate(2024, 3, 30);
        // Act
        string result = ForecastSubmitModel.GetDateRange(startDate, endDate);
        // Assert
        result.Should().Be($"Jan {expected} to Mar 30th 2024");
    }

    [Test]
    public void Operator_Implicit_Converts_ProviderCourseForecast_To_ForecastModel()
    {
        // Arrange
        var providerCourseForecast = new Application.ProviderCourseForecasts.Queries.GetProviderCourseForecasts.ProviderCourseForecast
        {
            TimePeriod = "2526",
            Quarter = 1,
            StartDate = GetDate(2026, 1, 1),
            EndDate = GetDate(2026, 3, 30),
            EstimatedLearners = 100
        };
        // Act
        ForecastSubmitModel forecastModel = providerCourseForecast;
        // Assert
        forecastModel.TimePeriod.Should().Be("2526");
        forecastModel.Quarter.Should().Be(1);
        forecastModel.Description.Should().Be("Jan 1st to Mar 30th 2026");
        forecastModel.EstimatedLearners.Should().Be(100);
        forecastModel.Id.Should().Be("2526-1");
    }

    [Test, AutoData]
    public void Operator_Converts_ForecastSubmitModel_To_UpsertForecastModel(ForecastSubmitModel expected)
    {
        UpsertForecastModel actual = expected;
        actual.Quarter.Should().Be(expected.Quarter);
        actual.TimePeriod.Should().Be(expected.TimePeriod);
        actual.EstimatedLearners.Should().Be(expected.EstimatedLearners);
    }
}
