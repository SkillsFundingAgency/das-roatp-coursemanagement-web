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
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllStandardRegions;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.EditShortCourseRegionsControllerTests;
public class EditShortCourseRegionsControllerPostTests
{
    [Test, MoqAutoData]
    public async Task EditShortCourseRegionsController_InvalidModelState_ReturnsView(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] EditShortCourseRegionsController sut,
            RegionsSubmitModel model,
            GetAllStandardRegionsQueryResult queryResult,
            string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "error");

        mediatorMock.Setup(m => m.Send(It.Is<GetAllStandardRegionsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        // Act
        var result = await sut.EditShortCourseRegions(model, apprenticeshipType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        var viewModel = viewResult.Model as SelectShortCourseRegionsViewModel;
        viewModel.SubregionsGroupedByRegions.Should().NotBeEmpty();
        viewModel.ShortCourseBaseModel.ApprenticeshipType.Should().Be(apprenticeshipType);
        viewModel.SubmitButtonText.Should().Be(ButtonText.Confirm);
        viewModel.Route.Should().Be(RouteNames.EditShortCourseRegions);
        viewModel.IsAddJourney.Should().BeFalse();
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseRegionsController_InvalidModelState_VerifyMediatorsAreInvokedCorrectly(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] EditShortCourseRegionsController sut,
            RegionsSubmitModel model,
            GetAllStandardRegionsQueryResult queryResult,
            string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "error");

        mediatorMock.Setup(m => m.Send(It.Is<GetAllStandardRegionsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        // Act
        await sut.EditShortCourseRegions(model, apprenticeshipType, larsCode);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<GetAllStandardRegionsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once());
        mediatorMock.Verify(m => m.Send(It.IsAny<UpdateStandardSubRegionsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseRegionsController_InvalidModelState_GetAllStandardRegionsReturnsNull_RedirectsToPageNotFound(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] EditShortCourseRegionsController sut,
            RegionsSubmitModel model,
            string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "error");

        mediatorMock.Setup(m => m.Send(It.Is<GetAllStandardRegionsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(() => null);

        // Act
        var result = await sut.EditShortCourseRegions(model, apprenticeshipType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseRegionsController_ValidState_GetAllStandardRegionsReturnsNull_VerifyMediatorsAreInvokedCorrectlyAndRedirectedToEditShortCourseRegions(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] EditShortCourseRegionsController sut,
        RegionsSubmitModel model,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetAllStandardRegionsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(() => null);

        // Act
        var result = await sut.EditShortCourseRegions(model, apprenticeshipType, larsCode);

        // Assert
        var routeResult = result as RedirectToRouteResult;
        routeResult.RouteName.Should().Be(RouteNames.EditShortCourseRegions);
        mediatorMock.Verify(m => m.Send(It.Is<GetAllStandardRegionsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once());
        mediatorMock.Verify(m => m.Send(It.IsAny<UpdateStandardSubRegionsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseRegionsController_ChangeToRegions_SendsUpdateCommandAndVerifyMediatorIsInvoked(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] EditShortCourseRegionsController sut,
        RegionsSubmitModel model,
        GetAllStandardRegionsQueryResult queryResult,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        string[] selectedSubRegions = new string[] { "1", "2", "3" };
        model.SelectedSubRegions = selectedSubRegions;

        queryResult.Regions = new List<CourseRegionModel>()
        {
            new CourseRegionModel()
            {
                Id = 1,
                IsSelected = true
            },
            new CourseRegionModel()
            {
                Id = 2,
                IsSelected = true
            }
        };

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetAllStandardRegionsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        // Act
        var result = await sut.EditShortCourseRegions(model, apprenticeshipType, larsCode);

        // Assert
        var routeResult = result as RedirectToRouteResult;
        routeResult.RouteName.Should().Be(RouteNames.ManageShortCourseDetails);
        mediatorMock.Verify(m => m.Send(It.Is<UpdateStandardSubRegionsCommand>(c => c.Ukprn == int.Parse(TestConstants.DefaultUkprn) && c.UserId == TestConstants.DefaultUserId && c.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseRegionsController_NoChangeToRegions_VerifyMediatorIsNotInvokedAndRedirectsToManageShortCourseDetails(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] EditShortCourseRegionsController sut,
        RegionsSubmitModel model,
        GetAllStandardRegionsQueryResult queryResult,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        string[] selectedSubRegions = new string[] { "1", "2" };
        model.SelectedSubRegions = selectedSubRegions;

        queryResult.Regions = new List<CourseRegionModel>()
        {
            new CourseRegionModel()
            {
                Id = 1,
                IsSelected = true
            },
            new CourseRegionModel()
            {
                Id = 2,
                IsSelected = true
            }
        };

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetAllStandardRegionsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        // Act
        var result = await sut.EditShortCourseRegions(model, apprenticeshipType, larsCode);

        // Assert
        var routeResult = result as RedirectToRouteResult;
        routeResult.RouteName.Should().Be(RouteNames.ManageShortCourseDetails);
        mediatorMock.Verify(m => m.Send(It.Is<UpdateStandardSubRegionsCommand>(c => c.Ukprn == int.Parse(TestConstants.DefaultUkprn) && c.UserId == TestConstants.DefaultUserId && c.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Never);
    }
}
