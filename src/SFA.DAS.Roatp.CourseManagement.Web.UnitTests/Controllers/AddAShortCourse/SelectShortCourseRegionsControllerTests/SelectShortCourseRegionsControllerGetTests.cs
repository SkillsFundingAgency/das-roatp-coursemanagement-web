using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.SelectShortCourseRegionsControllerTests;
public class SelectShortCourseRegionsControllerGetTests
{
    [Test, MoqAutoData]
    public async Task SelectShortCourseRegions_SessionIsValid_ReturnsView(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IRegionsService> regionsService,
        [Greedy] SelectShortCourseRegionsController sut,
        ShortCourseSessionModel sessionModel,
        List<RegionModel> regions
    )
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sessionModel.LocationOptions = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.EmployerLocation };

        sessionModel.HasNationalDeliveryOption = false;

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        regionsService.Setup(m => m.GetRegions()).ReturnsAsync(regions);

        // Act
        var result = await sut.SelectShortCourseRegions(apprenticeshipType);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult!.Model as SelectShortCourseRegionsViewModel;
        model!.SubregionsGroupedByRegions.Should().NotBeEmpty();
        model.ApprenticeshipType.Should().Be(apprenticeshipType);
        model.IsAddJourney.Should().BeTrue();
        model.Route.Should().Be(RouteNames.SelectShortCourseRegions);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        regionsService.Verify(m => m.GetRegions(), Times.Once());
    }

    [Test]
    [MoqInlineAutoData(false, ButtonText.Continue)]
    [MoqInlineAutoData(true, ButtonText.Confirm)]
    public async Task SelectShortCourseRegions_HasSeenSummaryPageIsTrueOrFalse_ReturnsExpectedButtonText(
        bool seenSummaryPage,
        string expectedSubmitButtonText,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IRegionsService> regionsService,
        [Greedy] SelectShortCourseRegionsController sut,
        ShortCourseSessionModel sessionModel,
        List<RegionModel> regions
    )
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sessionModel.LocationOptions = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.EmployerLocation };

        sessionModel.HasNationalDeliveryOption = false;
        sessionModel.HasSeenSummaryPage = seenSummaryPage;

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        regionsService.Setup(m => m.GetRegions()).ReturnsAsync(regions);

        // Act
        var result = await sut.SelectShortCourseRegions(apprenticeshipType);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult!.Model as SelectShortCourseRegionsViewModel;
        model!.SubmitButtonText.Should().Be(expectedSubmitButtonText);
    }

    [Test, MoqAutoData]
    public async Task SelectShortCourseRegions_SessionIsNull_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IRegionsService> regionsService,
        [Greedy] SelectShortCourseRegionsController sut)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = await sut.SelectShortCourseRegions(apprenticeshipType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        regionsService.Verify(m => m.GetRegions(), Times.Never());
    }

    [Test, MoqAutoData]
    public async Task SelectShortCourseRegions_EmployerLocationNotInSession_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IRegionsService> regionsService,
        [Greedy] SelectShortCourseRegionsController sut,
        ShortCourseSessionModel sessionModel
    )
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sessionModel.LocationOptions = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.Online };

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.SelectShortCourseRegions(apprenticeshipType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        regionsService.Verify(m => m.GetRegions(), Times.Never());
    }

    [Test, MoqAutoData]
    public async Task SelectShortCourseRegions_HasNationalDeliveryOptionIsTrueInSession_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IRegionsService> regionsService,
        [Greedy] SelectShortCourseRegionsController sut,
        ShortCourseSessionModel sessionModel
    )
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sessionModel.LocationOptions = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.EmployerLocation };

        sessionModel.HasNationalDeliveryOption = true;

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.SelectShortCourseRegions(apprenticeshipType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        regionsService.Verify(m => m.GetRegions(), Times.Never());
    }
}
