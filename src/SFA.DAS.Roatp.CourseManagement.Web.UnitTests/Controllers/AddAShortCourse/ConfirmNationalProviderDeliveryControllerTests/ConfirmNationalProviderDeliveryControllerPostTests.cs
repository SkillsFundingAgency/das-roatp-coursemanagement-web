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

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.ConfirmNationalProviderDeliveryControllerTests;
public class ConfirmNationalProviderDeliveryControllerPostTests
{
    [Test, MoqAutoData]
    public void ConfirmNationalProviderDelivery_InvalidState_ReturnsView(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmNationalProviderDeliveryController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        sut.ModelState.AddModelError("key", "message");

        // Act
        var response = sut.ConfirmNationalProviderDelivery(new ConfirmNationalProviderDeliverySubmitModel(), apprenticeshipType);

        // Assert
        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as ConfirmNationalProviderDeliveryViewModel;
        model.Should().NotBeNull();
        model!.ApprenticeshipType.Should().Be(apprenticeshipType);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void ConfirmNationalProviderDelivery_SessionIsNull_RedirectsToReviewYourDetails(
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Greedy] ConfirmNationalProviderDeliveryController sut)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = sut.ConfirmNationalProviderDelivery(new ConfirmNationalProviderDeliverySubmitModel(), apprenticeshipType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }

    [Test, MoqAutoData]
    public void ConfirmNationalProviderDelivery_HasNationalDeliveryOptionIsTrue_SetsSessionAndRedirectsToConfirmNationalProviderDelivery(
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Greedy] ConfirmNationalProviderDeliveryController sut,
    ShortCourseSessionModel sessionModel,
    ConfirmNationalProviderDeliverySubmitModel submitModel)
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
        redirectResult!.RouteName.Should().Be(RouteNames.ConfirmNationalProviderDelivery);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.HasNationalDeliveryOption == submitModel.HasNationalDeliveryOption)), Times.Once);
    }

    [Test, MoqAutoData]
    public void ConfirmNationalProviderDelivery_HasNationalDeliveryOptionIsFalse_SetsSessionAndRedirectsToConfirmNationalProviderDelivery(
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Greedy] ConfirmNationalProviderDeliveryController sut,
    ShortCourseSessionModel sessionModel,
    ConfirmNationalProviderDeliverySubmitModel submitModel)
    {
        // Arrange
        submitModel.HasNationalDeliveryOption = false;
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.ConfirmNationalProviderDelivery(submitModel, apprenticeshipType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.ConfirmNationalProviderDelivery);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.HasNationalDeliveryOption == submitModel.HasNationalDeliveryOption)), Times.Once);
    }
}
