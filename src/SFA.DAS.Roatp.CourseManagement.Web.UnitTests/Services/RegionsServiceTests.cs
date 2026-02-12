using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Regions.Queries;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Services;
public class RegionsServiceTests
{
    [Test, MoqAutoData]
    public async Task GetRegions_PresentInSession_ReturnFromSession(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        List<RegionModel> sessionRegions,
        RegionsService sut)
    {
        // Arrange
        sessionServiceMock.Setup(x => x.Get<List<RegionModel>>())
            .Returns(sessionRegions);

        // Act
        var result = await sut.GetRegions();

        // Assert
        result.Should().BeEquivalentTo(sessionRegions);
        distributedCacheServiceMock.Verify(x => x.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<List<RegionModel>>>>(), It.IsAny<TimeSpan>()), Times.Never);
        mediatorMock.Verify(x => x.Send(It.IsAny<GetAllRegionsAndSubRegionsQuery>(), CancellationToken.None), Times.Never);
    }


    [Test, MoqAutoData]
    public async Task GetRegions_SessionIsEmpty_ReturnFromCache(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        List<RegionModel> cachedRegions,
        RegionsService sut)
    {
        // Arrange
        sessionServiceMock.Setup(x => x.Get<List<RegionModel>>())
            .Returns(() => null);

        distributedCacheServiceMock
            .Setup(x => x.GetOrSetAsync(
                CacheSetting.Regions.Key,
                It.IsAny<Func<Task<List<RegionModel>>>>(),
                CacheSetting.Regions.CacheDuration))
            .ReturnsAsync(cachedRegions);

        // Act
        var result = await sut.GetRegions();

        // Assert
        result.Should().BeEquivalentTo(cachedRegions);
        sessionServiceMock.Verify(x => x.Set(cachedRegions), Times.Once);
        mediatorMock.Verify(x => x.Send(It.IsAny<GetAllRegionsAndSubRegionsQuery>(), CancellationToken.None), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task GetRegions_SessionAndCacheIsEmpty_ReturnFromApi(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IDistributedCacheService> distributedCacheServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        GetAllRegionsAndSubRegionsQueryResult apiResponse,
        RegionsService sut)
    {
        // Arrange
        var expectedRegions = new List<RegionModel>();
        apiResponse.Regions = expectedRegions;

        sessionServiceMock.Setup(x => x.Get<List<RegionModel>>())
            .Returns(() => null);

        distributedCacheServiceMock
            .Setup(x => x.GetOrSetAsync(
                CacheSetting.Regions.Key,
                It.IsAny<Func<Task<List<RegionModel>>>>(),
                CacheSetting.Regions.CacheDuration))
            .Returns<string, Func<Task<List<RegionModel>>>, TimeSpan>(async (key, factory, duration) =>
            {
                return await factory();
            });

        mediatorMock.Setup(x => x.Send(It.IsAny<GetAllRegionsAndSubRegionsQuery>(), CancellationToken.None)).ReturnsAsync(apiResponse);

        // Act
        var result = await sut.GetRegions();

        // Assert
        result.Should().BeEquivalentTo(expectedRegions);
        sessionServiceMock.Verify(x => x.Set(expectedRegions), Times.Once);
        distributedCacheServiceMock.Verify(x =>
            x.GetOrSetAsync(CacheSetting.Regions.Key,
                            It.IsAny<Func<Task<List<RegionModel>>>>(),
                            CacheSetting.Regions.CacheDuration),
            Times.Once);
        mediatorMock.Verify(x => x.Send(It.IsAny<GetAllRegionsAndSubRegionsQuery>(), CancellationToken.None), Times.Once);
    }
}
