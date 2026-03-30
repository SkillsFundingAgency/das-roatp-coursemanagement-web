using System.Collections.Generic;
using System.Linq;
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

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.ConfirmNationalDeliveryControllerTests;
public class ConfirmNationalDeliveryControllerPostTests
{
    [Test, MoqAutoData]
    public void ConfirmNationalProviderDelivery_InvalidState_ReturnsView(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmNationalDeliveryController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        sut.ModelState.AddModelError("key", "message");

        // Act
        var response = sut.ConfirmNationalProviderDelivery(new ConfirmNationalDeliverySubmitModel(), apprenticeshipType);

        // Assert
        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as ConfirmNationalDeliveryViewModel;
        model.Should().NotBeNull();
        model!.ApprenticeshipType.Should().Be(apprenticeshipType);
        model!.IsAddJourney.Should().BeTrue();
        model!.Route.Should().Be(RouteNames.ConfirmNationalDelivery);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test]
    [MoqInlineAutoData(false, ButtonText.Continue)]
    [MoqInlineAutoData(true, ButtonText.Confirm)]
    public void ConfirmNationalProviderDelivery_HasSeenSummaryPageIsTrueOrFalse_ReturnsExpectedButtonText(
    bool seenSummaryPage,
    string expectedSubmitButtonText,
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Greedy] ConfirmNationalDeliveryController sut,
    ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        sessionModel.HasSeenSummaryPage = seenSummaryPage;
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        sut.ModelState.AddModelError("key", "message");

        // Act
        var response = sut.ConfirmNationalProviderDelivery(new ConfirmNationalDeliverySubmitModel(), apprenticeshipType);

        // Assert
        var viewResult = response as ViewResult;
        var model = viewResult.Model as ConfirmNationalDeliveryViewModel;
        model!.SubmitButtonText.Should().Be(expectedSubmitButtonText);
    }

    [Test, MoqAutoData]
    public void ConfirmNationalProviderDelivery_SessionIsNull_RedirectsToReviewYourDetails(
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Greedy] ConfirmNationalDeliveryController sut)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = sut.ConfirmNationalProviderDelivery(new ConfirmNationalDeliverySubmitModel(), apprenticeshipType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }

    [Test, MoqAutoData]
    public void ConfirmNationalProviderDelivery_HasNationalDeliveryOptionIsTrue_SetsSessionAndRedirectsToReviewShortCourseDetails(
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Greedy] ConfirmNationalDeliveryController sut,
    ShortCourseSessionModel sessionModel,
    ConfirmNationalDeliverySubmitModel submitModel)
    {
        // Arrange
        submitModel.HasNationalDeliveryOption = true;
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.ConfirmNationalProviderDelivery(submitModel, apprenticeshipType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewShortCourseDetails);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.HasNationalDeliveryOption == submitModel.HasNationalDeliveryOption && m.TrainingRegions.SequenceEqual(new List<TrainingRegionModel>()))), Times.Once);
    }

    [Test, MoqAutoData]
    public void ConfirmNationalProviderDelivery_HasNationalDeliveryOptionIsFalse_SetsSessionAndRedirectsToSelectShortCourseRegions(
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Greedy] ConfirmNationalDeliveryController sut,
    ShortCourseSessionModel sessionModel,
    ConfirmNationalDeliverySubmitModel submitModel)
    {
        // Arrange
        submitModel.HasNationalDeliveryOption = false;
        sessionModel.TrainingRegions = new List<TrainingRegionModel>();
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.ConfirmNationalProviderDelivery(submitModel, apprenticeshipType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.SelectShortCourseRegions);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.HasNationalDeliveryOption == submitModel.HasNationalDeliveryOption)), Times.Once);
    }
}
