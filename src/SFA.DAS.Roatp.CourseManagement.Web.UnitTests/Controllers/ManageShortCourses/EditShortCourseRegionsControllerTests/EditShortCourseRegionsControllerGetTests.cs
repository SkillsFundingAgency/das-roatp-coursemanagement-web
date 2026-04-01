using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.EditShortCourseRegionsControllerTests;
public class EditShortCourseRegionsControllerGetTests
{
    [Test, MoqAutoData]
    public async Task EditShortCourseRegions_ProviderCourseExists_ReturnsView(
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<IRegionsService> regionsService,
    [Greedy] EditShortCourseRegionsController sut,
    List<RegionModel> regions,
    GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
    string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        regionsService.Setup(m => m.GetRegions()).ReturnsAsync(regions);

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.EditShortCourseRegions(apprenticeshipType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        var model = viewResult.Model as SelectShortCourseRegionsViewModel;
        model.SubregionsGroupedByRegions.Should().NotBeEmpty();
        model.ApprenticeshipType.Should().Be(apprenticeshipType);
        model.SubmitButtonText.Should().Be(ButtonText.Confirm);
        model.Route.Should().Be(RouteNames.EditShortCourseRegions);
        model.IsAddJourney.Should().BeFalse();
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseRegions_ProviderCourseExists_IsSelectedIsSetCorrectly(
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<IRegionsService> regionsService,
    [Greedy] EditShortCourseRegionsController sut,
    GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
    string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        List<RegionModel> regions = new List<RegionModel>()
        {
            new RegionModel()
            {
                Id = 1,
                SubregionName = "Test"
            },
            new RegionModel()
            {
                Id = 2,
                SubregionName = "Test2"
            }
        };

        providerCourseDetailsApiResponse.ProviderCourseLocations = new List<ProviderCourseLocation>()
        {
            new ProviderCourseLocation()
            {
                SubregionName = "Test",
                LocationType = LocationType.Regional
            },
            new ProviderCourseLocation()
            {
                SubregionName = "Test2",
                LocationType = LocationType.Regional
            }
        };

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        regionsService.Setup(m => m.GetRegions()).ReturnsAsync(regions);

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.EditShortCourseRegions(apprenticeshipType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult.Model as SelectShortCourseRegionsViewModel;
        model.SubregionsGroupedByRegions.FirstOrDefault().FirstOrDefault().IsSelected.Should().BeTrue();
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseRegions_ProviderCourseExists_VerifyMediatorIsInvoked(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IRegionsService> regionsService,
        [Greedy] EditShortCourseRegionsController sut,
        List<RegionModel> regions,
        GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        regionsService.Setup(m => m.GetRegions()).ReturnsAsync(regions);

        sut.AddDefaultContextWithUser();

        // Act
        await sut.EditShortCourseRegions(apprenticeshipType, larsCode);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once());
        regionsService.Verify(m => m.GetRegions(), Times.Once());
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseRegions_ProviderCourseDoesNotExist_RedirectsToPageNotFound(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] EditShortCourseRegionsController sut,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(() => null);

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.EditShortCourseRegions(apprenticeshipType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
    }
}
