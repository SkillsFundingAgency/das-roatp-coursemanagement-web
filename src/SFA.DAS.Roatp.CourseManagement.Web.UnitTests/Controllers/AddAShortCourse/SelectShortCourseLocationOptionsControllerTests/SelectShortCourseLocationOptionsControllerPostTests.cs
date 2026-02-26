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
    [Test]
    [MoqInlineAutoData(false, "Continue")]
    [MoqInlineAutoData(true, "Confirm")]
    public void SelectShortCourseLocation_InvalidState_ReturnsView(
        bool seenSummaryPage,
        string expectedSubmitButtonText,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationOptionsController sut,
        ShortCourseSessionModel sessionModel,
        SelectShortCourseLocationOptionsSubmitModel submitModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        sessionModel.HasSeenSummaryPage = seenSummaryPage;
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
        model!.SubmitButtonText.Should().Be(expectedSubmitButtonText);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.LocationOptions.FirstOrDefault() == submitModel.SelectedLocationOptions.FirstOrDefault() && m.HasOnlineDeliveryOption && m.TrainingVenues.SequenceEqual(new List<TrainingVenueModel>()) && m.HasNationalDeliveryOption == null && m.TrainingRegions.SequenceEqual(new List<TrainingRegionModel>()))), Times.Never());
    }

    [Test, MoqAutoData]
    public void SelectShortCourseLocation_OnlineOptionIsSelected_SetsSessionCorrectlyAndRedirectsToReviewShortCourseDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationOptionsController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sessionModel.HasSeenSummaryPage = false;

        var submitModel = new SelectShortCourseLocationOptionsSubmitModel() { SelectedLocationOptions = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.Online } };

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.AddDefaultContextWithUser();

        // Act
        var response = sut.SelectShortCourseLocation(submitModel, apprenticeshipType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewShortCourseDetails);
        sessionModel.HasOnlineDeliveryOption.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.LocationOptions.FirstOrDefault() == submitModel.SelectedLocationOptions.FirstOrDefault() && m.HasOnlineDeliveryOption && m.TrainingVenues.SequenceEqual(new List<TrainingVenueModel>()) && m.HasNationalDeliveryOption == null && m.TrainingRegions.SequenceEqual(new List<TrainingRegionModel>()))), Times.Once());
    }

    [Test, MoqAutoData]
    public void SelectShortCourseLocation_EmployerLocationOptionIsSelected_SetsSessionCorrectlyAndRedirectsToConfirmNationalProviderDelivery(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationOptionsController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sessionModel.HasSeenSummaryPage = false;

        var submitModel = new SelectShortCourseLocationOptionsSubmitModel() { SelectedLocationOptions = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.EmployerLocation } };

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.AddDefaultContextWithUser();

        // Act
        var response = sut.SelectShortCourseLocation(submitModel, apprenticeshipType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ConfirmNationalDelivery);
        sessionModel.HasOnlineDeliveryOption.Should().BeFalse();
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.LocationOptions.FirstOrDefault() == submitModel.SelectedLocationOptions.FirstOrDefault() && !m.HasOnlineDeliveryOption && m.TrainingVenues.SequenceEqual(new List<TrainingVenueModel>()) && m.HasNationalDeliveryOption == null && m.TrainingRegions.SequenceEqual(new List<TrainingRegionModel>()))), Times.Once());
    }

    [Test, MoqAutoData]
    public void SelectShortCourseLocation_ProviderLocationOptionIsSelected_SetsSessionCorrectlyAndRedirectsToSelectShortCourseTrainingVenue(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationOptionsController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sessionModel.HasSeenSummaryPage = false;

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
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.LocationOptions.FirstOrDefault() == submitModel.SelectedLocationOptions.FirstOrDefault() && !m.HasOnlineDeliveryOption && m.TrainingVenues.SequenceEqual(new List<TrainingVenueModel>()) && m.HasNationalDeliveryOption == null && m.TrainingRegions.SequenceEqual(new List<TrainingRegionModel>()))), Times.Once());
    }

    [Test, MoqAutoData]
    public void SelectShortCourseLocation_HasNationalDeliveryOptionIsFalseAndRegionsMissing_SetsSessionCorrectlyAndRedirectsToSelectShortCourseRegions(
       [Frozen] Mock<ISessionService> sessionServiceMock,
       [Greedy] SelectShortCourseLocationOptionsController sut,
       ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sessionModel.LocationOptions =
        [
            ShortCourseLocationOption.EmployerLocation
        ];

        sessionModel.HasSeenSummaryPage = false;

        sessionModel.HasNationalDeliveryOption = false;

        sessionModel.TrainingRegions = new List<TrainingRegionModel>();

        var submitModel = new SelectShortCourseLocationOptionsSubmitModel() { SelectedLocationOptions = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.EmployerLocation } };

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.AddDefaultContextWithUser();

        // Act
        var response = sut.SelectShortCourseLocation(submitModel, apprenticeshipType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.SelectShortCourseRegions);
        sessionModel.HasOnlineDeliveryOption.Should().BeFalse();
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.LocationOptions.FirstOrDefault() == submitModel.SelectedLocationOptions.FirstOrDefault() && !m.HasOnlineDeliveryOption)), Times.Once());
    }

    [Test, MoqAutoData]
    public void SelectShortCourseLocation_HasSeenSummaryPageIsTrueAndOptionsHaveBeenRemoved_RemovesSessionDataAndSetsSessionCorrectly(
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Greedy] SelectShortCourseLocationOptionsController sut,
    ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sessionModel.HasSeenSummaryPage = true;

        sessionModel.LocationOptions =
        [
            ShortCourseLocationOption.ProviderLocation,
            ShortCourseLocationOption.EmployerLocation,
            ShortCourseLocationOption.Online
        ];

        var submitModel = new SelectShortCourseLocationOptionsSubmitModel();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.AddDefaultContextWithUser();

        // Act
        sut.SelectShortCourseLocation(submitModel, apprenticeshipType);

        // Assert
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.LocationOptions.FirstOrDefault() == submitModel.SelectedLocationOptions.FirstOrDefault() && !m.HasOnlineDeliveryOption && m.TrainingVenues.SequenceEqual(new List<TrainingVenueModel>()) && m.HasNationalDeliveryOption == null && m.TrainingRegions.SequenceEqual(new List<TrainingRegionModel>()))), Times.Once());
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
