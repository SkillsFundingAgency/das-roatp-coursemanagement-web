using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Session;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.ConfirmSavedContactDetailsControllerTests;
public class ConfirmSavedContactDetailsControllerGetTests
{
    [Test, MoqAutoData]
    public void ConfirmSavedContactDetails_SessionIsValid_ReturnsView(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmSavedContactDetailsController sut,
        ShortCourseSessionModel sessionModel
    )
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.ConfirmSavedContactDetails(courseType);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult!.Model as ConfirmSavedContactDetailsViewModel;
        model!.EmailAddress.Should().Be(sessionModel.LatestProviderContactModel.EmailAddress);
        model.PhoneNumber.Should().Be(sessionModel.LatestProviderContactModel.PhoneNumber);
        model.ShowEmail.Should().Be(!string.IsNullOrWhiteSpace(sessionModel.LatestProviderContactModel.EmailAddress));
        model.ShowPhone.Should()
            .Be(!string.IsNullOrWhiteSpace(sessionModel.LatestProviderContactModel.PhoneNumber));
        model.IsUsingSavedContactDetails.Should().Be(sessionModel.IsUsingSavedContactDetails);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void ConfirmSavedContactDetails_SessionIsNull_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmSavedContactDetailsController sut)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = sut.ConfirmSavedContactDetails(courseType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }

    [Test, MoqAutoData]
    public void ConfirmSavedContactDetails_LatestProviderContactIsNullInSession_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmSavedContactDetailsController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sessionModel.LatestProviderContactModel = null;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.ConfirmSavedContactDetails(courseType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }

    [Test, MoqAutoData]
    public void ConfirmSavedContactDetails_EmailAddressAndPhoneNumberAreNullInSession_RedirectsToAddShortCourseContactDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmSavedContactDetailsController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sessionModel.LatestProviderContactModel = new ProviderContactModel()
        {
            EmailAddress = null,
            PhoneNumber = null
        };

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.ConfirmSavedContactDetails(courseType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.AddShortCourseContactDetails);
    }
}
