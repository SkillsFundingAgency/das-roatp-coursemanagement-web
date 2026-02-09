using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.ConfirmSavedContactDetailsControllerTests;
public class ConfirmSavedContactDetailsControllerPostTests
{
    [Test, MoqAutoData]
    public void ConfirmSavedContactDetails_InvalidState_ReturnsView(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmSavedContactDetailsController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        sut.ModelState.AddModelError("key", "message");

        // Act
        var response = sut.ConfirmSavedContactDetails(new ConfirmSavedContactDetailsSubmitModel(), apprenticeshipType);

        // Assert
        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as ConfirmSavedContactDetailsViewModel;
        model.Should().NotBeNull();
        model!.ApprenticeshipType.Should().Be(apprenticeshipType);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void ConfirmSavedContactDetails_ValidState_UseContactDetailsIsTrue_SetsContactDetailsInSessionAndRedirectsToAddShortCourseContactDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmSavedContactDetailsController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        var submitModel = new ConfirmSavedContactDetailsSubmitModel() { IsUsingSavedContactDetails = true };

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.AddDefaultContextWithUser();

        // Act
        var response = sut.ConfirmSavedContactDetails(submitModel, apprenticeshipType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.AddShortCourseContactDetails);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.IsUsingSavedContactDetails == submitModel.IsUsingSavedContactDetails && m.ContactInformation!.ContactUsEmail == sessionModel.SavedProviderContactModel.EmailAddress && m.ContactInformation!.ContactUsPhoneNumber == sessionModel.SavedProviderContactModel.PhoneNumber)), Times.Once);
    }

    [Test, MoqAutoData]
    public void ConfirmSavedContactDetails_ValidState_UseContactDetailsIsFalse_DoesNotSetContactDetailsInSessionAndRedirectsToAddShortCourseContactDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmSavedContactDetailsController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        var submitModel = new ConfirmSavedContactDetailsSubmitModel() { IsUsingSavedContactDetails = false };

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.AddDefaultContextWithUser();

        // Act
        var response = sut.ConfirmSavedContactDetails(submitModel, apprenticeshipType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.AddShortCourseContactDetails);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.IsUsingSavedContactDetails == submitModel.IsUsingSavedContactDetails)), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.IsUsingSavedContactDetails == submitModel.IsUsingSavedContactDetails && m.ContactInformation!.ContactUsEmail == sessionModel.SavedProviderContactModel.EmailAddress && m.ContactInformation!.ContactUsPhoneNumber == sessionModel.SavedProviderContactModel.PhoneNumber)), Times.Never);
    }

    [Test, MoqAutoData]
    public void ConfirmSavedContactDetails_SessionIsNull_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmSavedContactDetailsController sut)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = sut.ConfirmSavedContactDetails(new ConfirmSavedContactDetailsSubmitModel(), apprenticeshipType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<ShortCourseSessionModel>()), Times.Never);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }
}
