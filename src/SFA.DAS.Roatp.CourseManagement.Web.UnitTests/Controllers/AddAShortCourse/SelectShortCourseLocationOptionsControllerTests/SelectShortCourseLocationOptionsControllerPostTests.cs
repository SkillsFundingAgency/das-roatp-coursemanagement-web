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
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.SelectShortCourseLocationOptionsControllerTests;
public class SelectShortCourseLocationOptionsControllerPostTests
{
    [Test, MoqAutoData]
    public void SelectShortCourseLocation_InvalidState_ReturnsView(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationOptionsController sut,
        ShortCourseSessionModel sessionModel,
        SelectShortCourseLocationOptionsSubmitModel submitModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        var expectedLocationOptions = Enum.GetValues<ShortCourseLocationOption>().ToList();
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        sut.ModelState.AddModelError("key", "message");

        // Act
        var response = sut.SelectShortCourseLocation(submitModel, apprenticeshipType);

        // Assert
        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as SelectShortCourseLocationOptionsViewModel;
        model.Should().NotBeNull();
        model!.LocationOptions.Select(x => x.LocationOption).Should().BeEquivalentTo(expectedLocationOptions);
        model!.ApprenticeshipType.Should().Be(apprenticeshipType);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.LocationOptions.FirstOrDefault() == submitModel.SelectedLocationOptions.FirstOrDefault() && m.HasOnlineDeliveryOption == submitModel.SelectedLocationOptions.Contains(ShortCourseLocationOption.Online) && !m.TrainingVenues.FirstOrDefault().IsSelected)), Times.Never());
    }

    [Test, MoqAutoData]
    public void SelectShortCourseLocation_OnlineOptionIsSelected_SetsSessionCorrectlyAndRedirectsToSelectShortCourseLocation(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationOptionsController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        var submitModel = new SelectShortCourseLocationOptionsSubmitModel() { SelectedLocationOptions = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.Online } };

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.AddDefaultContextWithUser();

        // Act
        var response = sut.SelectShortCourseLocation(submitModel, apprenticeshipType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.SelectShortCourseLocationOption);
        sessionModel.HasOnlineDeliveryOption.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.LocationOptions.FirstOrDefault() == submitModel.SelectedLocationOptions.FirstOrDefault() && m.HasOnlineDeliveryOption == submitModel.SelectedLocationOptions.Contains(ShortCourseLocationOption.Online) && !m.TrainingVenues.FirstOrDefault().IsSelected)), Times.Once());
    }

    [Test, MoqAutoData]
    public void SelectShortCourseLocation_OnlineOptionIsNotSelected_SetsSessionCorrectlyAndRedirectsToSelectShortCourseLocation(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationOptionsController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        var submitModel = new SelectShortCourseLocationOptionsSubmitModel() { SelectedLocationOptions = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.EmployerLocation } };

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.AddDefaultContextWithUser();

        // Act
        var response = sut.SelectShortCourseLocation(submitModel, apprenticeshipType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.SelectShortCourseLocationOption);
        sessionModel.HasOnlineDeliveryOption.Should().BeFalse();
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.LocationOptions.FirstOrDefault() == submitModel.SelectedLocationOptions.FirstOrDefault() && m.HasOnlineDeliveryOption == submitModel.SelectedLocationOptions.Contains(ShortCourseLocationOption.Online) && !m.TrainingVenues.FirstOrDefault().IsSelected)), Times.Once());
    }

    [Test, MoqAutoData]
    public void SelectShortCourseLocation_ProviderLocationOptionIsSelected_SetsSessionCorrectlyAndRedirectsToSelectShortCourseTrainingVenue(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationOptionsController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        var submitModel = new SelectShortCourseLocationOptionsSubmitModel() { SelectedLocationOptions = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.ProviderLocation } };

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.AddDefaultContextWithUser();

        // Act
        var response = sut.SelectShortCourseLocation(submitModel, apprenticeshipType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.SelectShortCourseTrainingVenue);
        sessionModel.HasOnlineDeliveryOption.Should().BeFalse();
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.LocationOptions.FirstOrDefault() == submitModel.SelectedLocationOptions.FirstOrDefault() && m.HasOnlineDeliveryOption == submitModel.SelectedLocationOptions.Contains(ShortCourseLocationOption.Online) && !m.TrainingVenues.FirstOrDefault().IsSelected)), Times.Once());
    }

    [Test, MoqAutoData]
    public void SelectShortCourseLocation_SessionIsNull_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationOptionsController sut)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = sut.SelectShortCourseLocation(new SelectShortCourseLocationOptionsSubmitModel(), apprenticeshipType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<ShortCourseSessionModel>()), Times.Never);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }
}
