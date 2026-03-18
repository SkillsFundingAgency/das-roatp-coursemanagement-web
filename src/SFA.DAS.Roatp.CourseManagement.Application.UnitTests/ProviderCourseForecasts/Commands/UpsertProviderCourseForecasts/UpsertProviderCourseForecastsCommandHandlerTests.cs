using System.Collections.Generic;
using System.Threading;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderCourseForecasts.Commands.UpsertProviderCourseForecasts;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderCourseForecasts.Commands.UpsertProviderCourseForecasts;

public class UpsertProviderCourseForecastsCommandHandlerTests
{
    [Test]
    public void Handle_Calls_ApiClient_With_Correct_Parameters()
    {
        // Arrange
        var apiClientMock = new Mock<IApiClient>();
        var handler = new UpsertProviderCourseForecastsCommandHandler(apiClientMock.Object);
        var command = new UpsertProviderCourseForecastsCommand
        (
            12345678,
            "LARS123",
            new List<UpsertForecastModel>
            {
                new UpsertForecastModel("2526", 1, 100),
                new UpsertForecastModel("2526", 2, 150)
            }
        );
        // Act
        handler.Handle(command, CancellationToken.None).Wait();
        // Assert
        apiClientMock.Verify(x => x.Post(
            "providers/12345678/courses/LARS123/forecasts",
            command.Forecasts),
            Times.Once);
    }
}
