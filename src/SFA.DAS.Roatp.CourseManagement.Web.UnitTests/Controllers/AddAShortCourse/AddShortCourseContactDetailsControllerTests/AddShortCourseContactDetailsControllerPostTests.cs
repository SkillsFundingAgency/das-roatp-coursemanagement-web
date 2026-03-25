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

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.AddShortCourseContactDetailsControllerTests;
public class AddShortCourseContactDetailsControllerPostTests
{
    [Test]
    [MoqInlineAutoData(false, "Continue")]
    [MoqInlineAutoData(true, "Confirm")]
    public void AddShortCourseContactDetails_InvalidState_ReturnsView(
        bool seenSummaryPage,
        string expectedSubmitButtonText,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] AddShortCourseContactDetailsController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        sessionModel.HasSeenSummaryPage = seenSummaryPage;
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        sut.ModelState.AddModelError("key", "message");

        // Act
        var response = sut.AddShortCourseContactDetails(new CourseContactDetailsSubmitModel(), apprenticeshipType);

        // Assert
        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as ShortCourseContactDetailsViewModel;
        model.Should().NotBeNull();
        model!.ContactUsEmail.Should().Be(sessionModel.ContactInformation.ContactUsEmail);
        model!.ContactUsPhoneNumber.Should().Be(sessionModel.ContactInformation.ContactUsPhoneNumber);
        model!.StandardInfoUrl.Should().Be(sessionModel.ContactInformation.StandardInfoUrl);
        model!.ShowSavedContactDetailsText.Should().Be(sessionModel.IsUsingSavedContactDetails == true);
        model!.SubmitButtonText.Should().Be(expectedSubmitButtonText);
        model!.IsAddJourney.Should().BeTrue();
        model!.ApprenticeshipType.Should().Be(apprenticeshipType);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void AddShortCourseContactDetails_SessionIsNull_RedirectsToReviewYourDetails(
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Greedy] AddShortCourseContactDetailsController sut)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = sut.AddShortCourseContactDetails(new CourseContactDetailsSubmitModel(), apprenticeshipType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }

    [Test, MoqAutoData]
    public void AddShortCourseContactDetails_ValidState_SetsSessionAndRedirectsToSelectShortCourseLocation(
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Greedy] AddShortCourseContactDetailsController sut,
    ShortCourseSessionModel sessionModel,
    CourseContactDetailsSubmitModel submitModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        sessionModel.HasSeenSummaryPage = false;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.AddShortCourseContactDetails(submitModel, apprenticeshipType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.SelectShortCourseLocationOption);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.ContactInformation.ContactUsEmail == submitModel.ContactUsEmail && m.ContactInformation.ContactUsPhoneNumber == submitModel.ContactUsPhoneNumber && m.ContactInformation.StandardInfoUrl == submitModel.StandardInfoUrl)), Times.Once);
    }

    [Test, MoqAutoData]
    public void AddShortCourseContactDetails_HasSeenSummaryPageIsTrue_SetsSessionAndRedirectsToReviewShortCourseDetails(
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Greedy] AddShortCourseContactDetailsController sut,
    ShortCourseSessionModel sessionModel,
    CourseContactDetailsSubmitModel submitModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        sessionModel.HasSeenSummaryPage = true;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.AddShortCourseContactDetails(submitModel, apprenticeshipType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewShortCourseDetails);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.ContactInformation.ContactUsEmail == submitModel.ContactUsEmail && m.ContactInformation.ContactUsPhoneNumber == submitModel.ContactUsPhoneNumber && m.ContactInformation.StandardInfoUrl == submitModel.StandardInfoUrl)), Times.Once);
    }
}
