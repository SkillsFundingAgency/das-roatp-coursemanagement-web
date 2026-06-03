using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Roatp.CourseManagement.Infrastructure.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Infrastructure.UnitTests.Services;

public class ProviderCourseDetailsCachedServiceTests
{
    [Test, MoqAutoData]
    public async Task WhenServiceCalledOnce_ThenReturnDataFromApiAndVerifyApiInvokedOnce(
        [Frozen] Mock<IApiClient> apiClientMock,
        StandardDetails apiResponse,
        ProviderCourseDetailsCachedService sut,
        int ukprn,
        string larsCode)
    {
        // Arrange
        apiClientMock.Setup(x => x.Get<StandardDetails>($"providers/{ukprn}/courses/{larsCode}")).ReturnsAsync(apiResponse);

        // Act
        var result = await sut.GetCachedProviderCourseDetails(ukprn, larsCode);

        // Assert
        result.Should().BeEquivalentTo(apiResponse);
        apiClientMock.Verify(x => x.Get<StandardDetails>($"providers/{ukprn}/courses/{larsCode}"), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenServiceCalledMultipleTimes_Then_ReturnCachedDataAndVerifyApiInvokedOnce(
        [Frozen] Mock<IApiClient> apiClientMock,
        StandardDetails apiResponse,
        ProviderCourseDetailsCachedService sut,
        int ukprn,
        string larsCode)
    {
        // Arrange
        apiClientMock.Setup(x => x.Get<StandardDetails>($"providers/{ukprn}/courses/{larsCode}")).ReturnsAsync(apiResponse);

        // Act
        var firstResult = await sut.GetCachedProviderCourseDetails(ukprn, larsCode);
        var secondResult = await sut.GetCachedProviderCourseDetails(ukprn, larsCode);

        // Assert
        firstResult.Should().BeEquivalentTo(apiResponse);
        secondResult.Should().BeEquivalentTo(apiResponse);
        apiClientMock.Verify(x => x.Get<StandardDetails>($"providers/{ukprn}/courses/{larsCode}"), Times.Once);
    }
}
