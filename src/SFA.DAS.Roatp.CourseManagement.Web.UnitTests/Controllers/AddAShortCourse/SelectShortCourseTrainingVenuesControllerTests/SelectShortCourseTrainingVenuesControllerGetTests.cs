using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.SelectShortCourseTrainingVenuesControllerTests;
public class SelectShortCourseTrainingVenuesControllerGetTests
{
    [Test, MoqAutoData]
    public void SelectShortCourseTrainingVenue_SessionIsValid_ReturnsView(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseTrainingVenuesController sut,
        ShortCourseSessionModel sessionModel
    )
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.SelectShortCourseTrainingVenue(courseType);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult!.Model as SelectShortCourseTrainingVenuesViewModel;
        model!.TrainingVenues.Should().BeEquivalentTo(sessionModel.TrainingVenues);
        model.CourseType.Should().Be(courseType);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void SelectShortCourseTrainingVenue_SessionIsNull_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseTrainingVenuesController sut)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = sut.SelectShortCourseTrainingVenue(courseType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }

    [Test, MoqAutoData]
    public void SelectShortCourseTrainingVenue_AtProviderLocationNotSelected_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseTrainingVenuesController sut,
        ShortCourseSessionModel sessionModel
    )
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sessionModel.LocationOptions = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.Online };

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.SelectShortCourseTrainingVenue(courseType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }

    [Test, MoqAutoData]
    public void SelectShortCourseTrainingVenue_TrainingVenuesIsEmptyInSession_RedirectsToGetAddTrainingVenue(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseTrainingVenuesController sut,
        ShortCourseSessionModel sessionModel
    )
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sessionModel.LocationOptions = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.ProviderLocation };

        sessionModel.TrainingVenues = new List<TrainingVenueModel>();

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.SelectShortCourseTrainingVenue(courseType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.GetAddTrainingVenue);
    }
}
