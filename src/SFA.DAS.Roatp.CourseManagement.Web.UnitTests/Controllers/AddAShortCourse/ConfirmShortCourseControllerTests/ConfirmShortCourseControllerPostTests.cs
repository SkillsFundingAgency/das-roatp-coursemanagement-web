using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Session;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.ConfirmShortCourseControllerTests;
public class ConfirmShortCourseControllerPostTests
{
    [Test, MoqAutoData]
    public void ConfirmShortCourse_InvalidState_ReturnsView(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmShortCourseController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sut.ModelState.AddModelError("key", "message");

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var response = sut.ConfirmShortCourse(new ConfirmShortCourseSubmitModel() { CourseType = courseType }, courseType);

        // Assert
        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as ConfirmShortCourseViewModel;
        model.Should().NotBeNull();
        model!.ShortCourseInformation.Should().BeEquivalentTo(sessionModel.ShortCourseInformation, o => o.ExcludingMissingMembers());
        model!.CourseType.Should().Be(courseType);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Delete(nameof(ShortCourseSessionModel)), Times.Never);
    }

    [Test, MoqAutoData]
    public void ConfirmShortCourse_ValidState_IsCorrectShortCourseIsFalse_ClearsSessionAndRedirectsToSelectAnApprenticeshipUnit(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmShortCourseController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var response = sut.ConfirmShortCourse(new ConfirmShortCourseSubmitModel() { IsCorrectShortCourse = false, CourseType = courseType }, courseType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.SelectShortCourse);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Delete(nameof(ShortCourseSessionModel)), Times.Once);
    }

    [Test, MoqAutoData]
    public void ConfirmShortCourse_ValidState_IsCorrectShortCourseIsTrue_RedirectsToConfirmSavedContactDetailsForShortCourse(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmShortCourseController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var response = sut.ConfirmShortCourse(new ConfirmShortCourseSubmitModel() { IsCorrectShortCourse = true, CourseType = courseType }, courseType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ConfirmSavedContactDetailsForShortCourse);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Delete(nameof(ShortCourseSessionModel)), Times.Never);
    }

    [Test, MoqAutoData]
    public void ConfirmShortCourse_SessionIsNull_RedirectsToReviewYourDetails(
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Greedy] ConfirmShortCourseController sut,
    ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(() => null);

        // Act
        var response = sut.ConfirmShortCourse(new ConfirmShortCourseSubmitModel() { CourseType = courseType }, courseType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Delete(nameof(ShortCourseSessionModel)), Times.Never);
    }

    [Test, MoqAutoData]
    public void ConfirmShortCourse_ValidState_LatestProviderContactIsNullInSession_RedirectsToConfirmSavedContactDetailsForShortCourse(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmShortCourseController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sessionModel.LatestProviderContactModel = null;

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var response = sut.ConfirmShortCourse(new ConfirmShortCourseSubmitModel() { IsCorrectShortCourse = true, CourseType = courseType }, courseType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ConfirmSavedContactDetailsForShortCourse);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Delete(nameof(ShortCourseSessionModel)), Times.Never);
    }
}
