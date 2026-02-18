using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.SelectShortCourseRegionsControllerTests;
public class SelectShortCourseRegionsControllerPostTests
{
    [Test, MoqAutoData]
    public async Task SelectShortCourseRegions_InvalidState_ReturnsView(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IRegionsService> regionsService,
        [Greedy] SelectShortCourseRegionsController sut,
        ShortCourseSessionModel sessionModel,
        RegionsSubmitModel submitModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = await sut.SelectShortCourseRegions(submitModel, apprenticeshipType);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult!.Model as SelectShortCourseRegionsViewModel;
        model!.SubregionsGroupedByRegions.Should().NotBeEmpty();
        model.ShortCourseBaseModel.ApprenticeshipType.Should().Be(apprenticeshipType);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        regionsService.Verify(m => m.GetRegions(), Times.Once());
    }

    [Test, MoqAutoData]
    public async Task SelectShortCourseRegions_IsValidState_SetsSessionCorrectlyAndRedirectsToSelectShortCourseRegions(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IRegionsService> regionsService,
        [Greedy] SelectShortCourseRegionsController sut,
        ShortCourseSessionModel sessionModel,
        List<RegionModel> regions)
    {
        // Arrange
        sessionModel.TrainingRegions = regions.Select(p => (TrainingRegionModel)p).ToList();
        sessionModel.LocationOptions =
        [
            ShortCourseLocationOption.EmployerLocation
        ];
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        var submitModel = new RegionsSubmitModel()
        {
            SelectedSubRegions = sessionModel.TrainingRegions.Select(l => l.SubregionId.ToString()).ToArray()
        };
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        regionsService.Setup(m => m.GetRegions()).ReturnsAsync(regions);

        // Act
        var response = await sut.SelectShortCourseRegions(submitModel, apprenticeshipType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.SelectShortCourseRegions);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.TrainingRegions.FirstOrDefault().SubregionId.ToString() == submitModel.SelectedSubRegions.FirstOrDefault())), Times.Once());
        regionsService.Verify(m => m.GetRegions(), Times.Once());
    }

    [Test, MoqAutoData]
    public async Task SelectShortCourseRegions_SessionIsNull_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IRegionsService> regionsService,
        [Greedy] SelectShortCourseRegionsController sut,
        RegionsSubmitModel submitModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = await sut.SelectShortCourseRegions(submitModel, apprenticeshipType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        regionsService.Verify(m => m.GetRegions(), Times.Never());
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }
}
