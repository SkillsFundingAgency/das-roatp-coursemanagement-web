using System;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Services;

public class ProviderCourseDetailsServiceTests
{
    [Test, MoqAutoData]
    public async Task WhenDataExistsInCache_Then_ReturnDataFromCache(
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        GetProviderCourseDetailsQueryResult cachedProviderCourseDetails,
        ProviderCourseDetailsService sut,
        int ukprn,
        string larsCode)
    {
        // Arrange
        var cacheKey = CacheKeys.GetProviderCourseDetailsKey(ukprn, larsCode);

        distributedCacheServiceMock
            .Setup(x => x.GetOrSetAsync(
                cacheKey,
                It.IsAny<Func<Task<GetProviderCourseDetailsQueryResult>>>(),
                CacheSetting.ProviderCourseDetails.CacheDuration))
            .ReturnsAsync(cachedProviderCourseDetails);

        // Act
        var result = await sut.GetProviderCourseDetails(ukprn, larsCode);

        // Assert
        result.Should().BeEquivalentTo(cachedProviderCourseDetails);
        mediatorMock.Verify(x => x.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn == ukprn && q.LarsCode == larsCode)), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task WhenCacheIsEmpty_Then_InvokeMediatorAndReturnDataFromApi(
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        GetProviderCourseDetailsQueryResult apiResponse,
        ProviderCourseDetailsService sut,
        int ukprn,
        string larsCode)
    {
        // Arrange
        var cacheKey = CacheKeys.GetProviderCourseDetailsKey(ukprn, larsCode);

        distributedCacheServiceMock
            .Setup(x => x.GetOrSetAsync(
                cacheKey,
                It.IsAny<Func<Task<GetProviderCourseDetailsQueryResult>>>(),
                CacheSetting.ProviderCourseDetails.CacheDuration))
            .Returns<string, Func<Task<GetProviderCourseDetailsQueryResult>>, TimeSpan>(async (key, factory, duration) =>
            {
                return await factory();
            });

        mediatorMock.Setup(x => x.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn == ukprn && q.LarsCode == larsCode))).ReturnsAsync(apiResponse);

        // Act
        var result = await sut.GetProviderCourseDetails(ukprn, larsCode);

        // Assert
        result.Should().BeEquivalentTo(apiResponse);
        distributedCacheServiceMock.Verify(x =>
            x.GetOrSetAsync(cacheKey,
                            It.IsAny<Func<Task<GetProviderCourseDetailsQueryResult>>>(),
                            CacheSetting.ProviderCourseDetails.CacheDuration),
            Times.Once);
        mediatorMock.Verify(x => x.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn == ukprn && q.LarsCode == larsCode)), Times.Once);
    }
}
