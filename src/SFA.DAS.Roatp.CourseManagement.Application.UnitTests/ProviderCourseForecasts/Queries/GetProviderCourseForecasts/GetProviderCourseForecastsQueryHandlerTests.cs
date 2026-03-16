using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderCourseForecasts.Queries.GetProviderCourseForecasts;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderCourseForecasts.Queries.GetProviderCourseForecasts;

public class GetProviderCourseForecastsQueryHandlerTests
{
    [Test]
    public void Handle_Returns_Forecasts_From_ApiClient()
    {
        // Arrange
        var apiClientMock = new Mock<IApiClient>();
        var handler = new GetProviderCourseForecastsQueryHandler(apiClientMock.Object, Mock.Of<ILogger<GetProviderCourseForecastsQueryHandler>>());
        var query = new GetProviderCourseForecastsQuery(12345678, "LARS123");
        var expectedResult = new GetProviderCourseForecastsQueryResult("LARS123", "Test Course", 3, new List<ProviderCourseForecast>
        {
            new ProviderCourseForecast
            {
                TimePeriod = "2526",
                Quarter = 1,
                StartDate = CommonHelpers.GetDate(2026, 1, 1),
                EndDate = CommonHelpers.GetDate(2026, 3, 30),
                EstimatedLearners = 100,
                UpdatedDate = DateTime.UtcNow
            }
        });
        apiClientMock.Setup(x => x.Get<GetProviderCourseForecastsQueryResult>("providers/12345678/courses/LARS123/forecasts"))
            .ReturnsAsync(expectedResult);
        // Act
        var result = handler.Handle(query, CancellationToken.None).Result;
        // Assert
        Assert.AreEqual(expectedResult, result);
    }
}
