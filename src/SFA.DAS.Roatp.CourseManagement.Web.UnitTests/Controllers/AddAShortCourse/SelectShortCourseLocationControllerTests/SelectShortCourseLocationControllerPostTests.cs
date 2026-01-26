using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Session;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.SelectShortCourseLocationControllerTests;
public class SelectShortCourseLocationControllerPostTests
{
    [Test, MoqAutoData]
    public void ConfirmSavedContactDetails_InvalidState_ReturnsView(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationController sut,
        ShortCourseSessionModel sessionModel,
        SelectShortCourseLocationSubmitModel submitModel)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        sut.ModelState.AddModelError("key", "message");

        // Act
        var response = sut.SelectShortCourseLocation(submitModel, courseType);

        // Assert
        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as SelectShortCourseLocationViewModel;
        model.Should().NotBeNull();
        model!.ShortCourseLocations.Should().BeEquivalentTo(submitModel.ShortCourseLocations);
        model!.CourseType.Should().Be(courseType);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.ShortCourseLocations == submitModel.ShortCourseLocations && m.HasOnlineDeliveryOption == submitModel.ShortCourseLocations.Contains(ShortCourseLocationOption.Online))), Times.Never);
    }

    [Test, MoqAutoData]
    public void SelectShortCourseLocation_ValidState_OnlineOptionIsSelected_SetsSessionCorrectlyAndRedirectsToSelectShortCourseLocation(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        var submitModel = new SelectShortCourseLocationSubmitModel() { ShortCourseLocations = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.Online } };

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.AddDefaultContextWithUser();

        // Act
        var response = sut.SelectShortCourseLocation(submitModel, courseType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.SelectShortCourseLocation);
        sessionModel.HasOnlineDeliveryOption.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.ShortCourseLocations == submitModel.ShortCourseLocations && m.HasOnlineDeliveryOption == submitModel.ShortCourseLocations.Contains(ShortCourseLocationOption.Online))), Times.Once);
    }

    [Test, MoqAutoData]
    public void SelectShortCourseLocation_ValidState_OnlineOptionIsNotSelected_SetsSessionCorrectlyAndRedirectsToSelectShortCourseLocation(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        var submitModel = new SelectShortCourseLocationSubmitModel() { ShortCourseLocations = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.ProviderLocation } };

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.AddDefaultContextWithUser();

        // Act
        var response = sut.SelectShortCourseLocation(submitModel, courseType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.SelectShortCourseLocation);
        sessionModel.HasOnlineDeliveryOption.Should().BeFalse();
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.ShortCourseLocations == submitModel.ShortCourseLocations && m.HasOnlineDeliveryOption == submitModel.ShortCourseLocations.Contains(ShortCourseLocationOption.Online))), Times.Once);
    }

    [Test, MoqAutoData]
    public void SelectShortCourseLocation_SessionIsNull_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationController sut)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = sut.SelectShortCourseLocation(new SelectShortCourseLocationSubmitModel(), courseType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<ShortCourseSessionModel>()), Times.Never);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }
}
