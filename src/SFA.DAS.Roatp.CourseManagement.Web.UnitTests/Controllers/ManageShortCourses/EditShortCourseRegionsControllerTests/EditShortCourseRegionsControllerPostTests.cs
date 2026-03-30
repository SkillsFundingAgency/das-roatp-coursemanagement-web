using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateStandardSubRegions;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.EditShortCourseRegionsControllerTests;
public class EditShortCourseRegionsControllerPostTests
{
    [Test, MoqAutoData]
    public async Task EditShortCourseRegions_InvalidModelState_ReturnsView(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<IRegionsService> regionsService,
            [Greedy] EditShortCourseRegionsController sut,
            RegionsSubmitModel model,
            List<RegionModel> regions,
            GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
            string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "error");

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        regionsService.Setup(m => m.GetRegions()).ReturnsAsync(regions);

        // Act
        var result = await sut.EditShortCourseRegions(model, apprenticeshipType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        var viewModel = viewResult.Model as SelectShortCourseRegionsViewModel;
        viewModel.SubregionsGroupedByRegions.Should().NotBeEmpty();
        viewModel.ApprenticeshipType.Should().Be(apprenticeshipType);
        viewModel.SubmitButtonText.Should().Be(ButtonText.Confirm);
        viewModel.Route.Should().Be(RouteNames.EditShortCourseRegions);
        viewModel.IsAddJourney.Should().BeFalse();
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseRegions_InvalidModelState_VerifyMediatorsAreInvokedCorrectly(
            [Frozen] Mock<IMediator> mediatorMock,
            [Frozen] Mock<IRegionsService> regionsService,
            [Greedy] EditShortCourseRegionsController sut,
            RegionsSubmitModel model,
            List<RegionModel> regions,
            string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "error");

        regionsService.Setup(m => m.GetRegions()).ReturnsAsync(regions);

        // Act
        await sut.EditShortCourseRegions(model, apprenticeshipType, larsCode);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Never());
        regionsService.Verify(m => m.GetRegions(), Times.Once());
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseRegions_ProviderCourseDoesNotExist_VerifyMediatorsAreInvokedCorrectlyAndRedirectedToEditShortCourseRegions(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IRegionsService> regionsService,
        [Greedy] EditShortCourseRegionsController sut,
        RegionsSubmitModel model,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(() => null);

        // Act
        var result = await sut.EditShortCourseRegions(model, apprenticeshipType, larsCode);

        // Assert
        var routeResult = result as RedirectToRouteResult;
        routeResult.RouteName.Should().Be(RouteNames.EditShortCourseRegions);
        mediatorMock.Verify(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once());
        regionsService.Verify(m => m.GetRegions(), Times.Never());
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseRegions_ChangeToRegions_SendsUpdateCommandAndVerifyMediatorIsInvoked(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IRegionsService> regionsService,
        [Greedy] EditShortCourseRegionsController sut,
        RegionsSubmitModel model,
        GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        string[] selectedSubRegions = new string[] { "1", "2", "3" };
        model.SelectedSubRegions = selectedSubRegions;

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
            },
            new RegionModel()
            {
                Id = 3,
                SubregionName = "Test3"
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

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        regionsService.Setup(m => m.GetRegions()).ReturnsAsync(regions);

        // Act
        var result = await sut.EditShortCourseRegions(model, apprenticeshipType, larsCode);

        // Assert
        var routeResult = result as RedirectToRouteResult;
        routeResult.RouteName.Should().Be(RouteNames.ManageShortCourseDetails);
        mediatorMock.Verify(m => m.Send(It.Is<UpdateStandardSubRegionsCommand>(c => c.Ukprn == int.Parse(TestConstants.DefaultUkprn) && c.UserId == TestConstants.DefaultUserId && c.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseRegions_NoChangeToRegions_VerifyMediatorIsNotInvokedAndRedirectsToManageShortCourseDetails(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IRegionsService> regionsService,
        [Greedy] EditShortCourseRegionsController sut,
        RegionsSubmitModel model,
        GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        string[] selectedSubRegions = new string[] { "1", "2" };
        model.SelectedSubRegions = selectedSubRegions;

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
            },
            new RegionModel()
            {
                Id = 3,
                SubregionName = "Test3"
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

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        regionsService.Setup(m => m.GetRegions()).ReturnsAsync(regions);

        // Act
        var result = await sut.EditShortCourseRegions(model, apprenticeshipType, larsCode);

        // Assert
        var routeResult = result as RedirectToRouteResult;
        routeResult.RouteName.Should().Be(RouteNames.ManageShortCourseDetails);
        mediatorMock.Verify(m => m.Send(It.Is<UpdateStandardSubRegionsCommand>(c => c.Ukprn == int.Parse(TestConstants.DefaultUkprn) && c.UserId == TestConstants.DefaultUserId && c.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Never);
    }
}
