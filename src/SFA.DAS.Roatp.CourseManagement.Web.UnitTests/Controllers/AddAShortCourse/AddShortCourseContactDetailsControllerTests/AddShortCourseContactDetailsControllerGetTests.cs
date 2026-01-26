using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Session;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.AddShortCourseContactDetailsControllerTests;
public class AddShortCourseContactDetailsControllerGetTests
{
    [Test, MoqAutoData]
    public void AddShortCourseContactDetails_ProviderContactDetailsHasValueInSession_ReturnsViewWithContactDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] AddShortCourseContactDetailsController sut,
        ShortCourseSessionModel sessionModel
    )
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.AddShortCourseContactDetails(courseType);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult!.Model as AddShortCourseContactDetailsViewModel;
        model!.ContactUsEmail.Should().Be(sessionModel.ContactInformation.ContactUsEmail);
        model!.ContactUsPhoneNumber.Should().Be(sessionModel.ContactInformation.ContactUsPhoneNumber);
        model!.StandardInfoUrl.Should().Be(sessionModel.ContactInformation.StandardInfoUrl);
        model!.CourseType.Should().Be(courseType);
        model!.ShowSavedContactDetailsText.Should().Be(sessionModel.IsUsingSavedContactDetails == true);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void AddShortCourseContactDetails_ProviderContactDetailsIsNullInSession_ReturnsViewWithEmptyFields(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] AddShortCourseContactDetailsController sut,
        ShortCourseSessionModel sessionModel
    )
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sessionModel.ContactInformation = new ContactInformationSessionModel();

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.AddShortCourseContactDetails(courseType);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult!.Model as AddShortCourseContactDetailsViewModel;
        model!.ContactUsEmail.Should().BeNull();
        model!.ContactUsPhoneNumber.Should().BeNull();
        model!.StandardInfoUrl.Should().BeNull();
        model!.CourseType.Should().Be(courseType);
        model!.ShowSavedContactDetailsText.Should().Be(sessionModel.IsUsingSavedContactDetails == true);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void AddShortCourseContactDetails_SessionIsNull_RedirectsToReviewYourDetails(
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Greedy] AddShortCourseContactDetailsController sut)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = sut.AddShortCourseContactDetails(courseType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }
}
