using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.SelectShortCourseTrainingVenuesControllerTests;
public class SelectShortCourseTrainingVenuesControllerPostTests
{
    [Test, MoqAutoData]
    public void SelectShortCourseTrainingVenue_InvalidState_ReturnsView(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseTrainingVenuesController sut,
        ShortCourseSessionModel sessionModel,
        SelectShortCourseTrainingVenuesSubmitModel submitModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        sessionModel.TrainingVenues = sessionModel.ProviderLocations.Select(p => (TrainingVenueModel)p).Where(p => p.LocationType == LocationType.Provider).ToList();
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        sut.ModelState.AddModelError("key", "message");

        // Act
        var response = sut.SelectShortCourseTrainingVenue(submitModel, apprenticeshipType);

        // Assert
        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as SelectShortCourseTrainingVenuesViewModel;
        model.Should().NotBeNull();
        model!.TrainingVenues.Should().BeEquivalentTo(sessionModel.TrainingVenues);
        model!.ApprenticeshipType.Should().Be(apprenticeshipType);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<ShortCourseSessionModel>()), Times.Never);
    }

    [Test, MoqAutoData]
    public void SelectShortCourseTrainingVenue_SetsSessionCorrectlyAndRedirectsToSelectReviewShortCourseDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseTrainingVenuesController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        sessionModel.TrainingVenues = sessionModel.ProviderLocations.Select(p => (TrainingVenueModel)p).Where(p => p.LocationType == LocationType.Provider).ToList();
        sessionModel.LocationOptions =
        [
            ShortCourseLocationOption.ProviderLocation
        ];
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        var submitModel = new SelectShortCourseTrainingVenuesSubmitModel()
        {
            SelectedProviderLocationIds = sessionModel.TrainingVenues.Select(l => l.ProviderLocationId).ToList(),
        };
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var response = sut.SelectShortCourseTrainingVenue(submitModel, apprenticeshipType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewShortCourseDetails);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.TrainingVenues.FirstOrDefault().ProviderLocationId == submitModel.SelectedProviderLocationIds.FirstOrDefault())), Times.Once());
    }

    [Test, MoqAutoData]
    public void SelectShortCourseTrainingVenue_SessionContainsEmployerLocationOption_SetsSessionCorrectlyAndRedirectsToConfirmNationalProviderDelivery(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseTrainingVenuesController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        sessionModel.TrainingVenues = sessionModel.ProviderLocations.Select(p => (TrainingVenueModel)p).Where(p => p.LocationType == LocationType.Provider).ToList();
        sessionModel.LocationOptions =
        [
            ShortCourseLocationOption.ProviderLocation,
            ShortCourseLocationOption.EmployerLocation
        ];
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        var submitModel = new SelectShortCourseTrainingVenuesSubmitModel()
        {
            SelectedProviderLocationIds = sessionModel.TrainingVenues.Select(l => l.ProviderLocationId).ToList(),
        };
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var response = sut.SelectShortCourseTrainingVenue(submitModel, apprenticeshipType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ConfirmNationalDelivery);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.TrainingVenues.FirstOrDefault().ProviderLocationId == submitModel.SelectedProviderLocationIds.FirstOrDefault())), Times.Once());
    }

    [Test, MoqAutoData]
    public void SelectShortCourseTrainingVenue_SessionIsNull_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseTrainingVenuesController sut,
        SelectShortCourseTrainingVenuesSubmitModel submitModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = sut.SelectShortCourseTrainingVenue(submitModel, apprenticeshipType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }
}
