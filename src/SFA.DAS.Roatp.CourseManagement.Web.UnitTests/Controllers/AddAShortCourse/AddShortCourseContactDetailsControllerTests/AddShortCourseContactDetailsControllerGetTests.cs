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

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.AddShortCourseContactDetailsControllerTests;
public class AddShortCourseContactDetailsControllerGetTests
{
    [Test]
    [MoqInlineAutoData(false, "Continue")]
    [MoqInlineAutoData(true, "Confirm")]
    public void AddShortCourseContactDetails_ProviderContactDetailsHasValueInSession_ReturnsViewWithContactDetails(
        bool seenSummaryPage,
        string expectedSubmitButtonText,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] AddShortCourseContactDetailsController sut,
        ShortCourseSessionModel sessionModel
    )
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        sessionModel.HasSeenSummaryPage = seenSummaryPage;

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.AddShortCourseContactDetails(apprenticeshipType);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult!.Model as AddShortCourseContactDetailsViewModel;
        model!.ContactUsEmail.Should().Be(sessionModel.ContactInformation.ContactUsEmail);
        model!.ContactUsPhoneNumber.Should().Be(sessionModel.ContactInformation.ContactUsPhoneNumber);
        model!.StandardInfoUrl.Should().Be(sessionModel.ContactInformation.StandardInfoUrl);
        model!.ApprenticeshipType.Should().Be(apprenticeshipType);
        model!.ShowSavedContactDetailsText.Should().Be(sessionModel.IsUsingSavedContactDetails == true);
        model!.SubmitButtonText.Should().Be(expectedSubmitButtonText);
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
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sessionModel.ContactInformation = new ContactInformationModel();

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = sut.AddShortCourseContactDetails(apprenticeshipType);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult!.Model as AddShortCourseContactDetailsViewModel;
        model!.ContactUsEmail.Should().BeNull();
        model!.ContactUsPhoneNumber.Should().BeNull();
        model!.StandardInfoUrl.Should().BeNull();
        model!.ApprenticeshipType.Should().Be(apprenticeshipType);
        model!.ShowSavedContactDetailsText.Should().Be(sessionModel.IsUsingSavedContactDetails == true);
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
        var result = sut.AddShortCourseContactDetails(apprenticeshipType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }
}
