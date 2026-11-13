using AutoFixture.NUnit4;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
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

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.ConfirmRegulatedShortCourseControllerTests;

public class ConfirmRegulatedShortCourseControllerPostTests
{
    [Test, MoqAutoData]
    public void WhenInvalidState_ThenReturnsView(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmRegulatedShortCourseController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var learningType = LearningType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sut.ModelState.AddModelError("key", "message");

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var response = sut.ConfirmRegulatedShortCourse(new ConfirmRegulatedShortCourseSubmitModel() { LearningType = learningType }, learningType);

        // Assert
        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as ConfirmRegulatedShortCourseViewModel;
        model.Should().NotBeNull();
        model!.ShortCourseInformation.Should().BeEquivalentTo(sessionModel.ShortCourseInformation, o => o.ExcludingMissingMembers());
        model!.LearningType.Should().Be(learningType);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Delete(nameof(ShortCourseSessionModel)), Times.Never);
    }

    [Test, MoqAutoData]
    public void WhenIsApprovedByRegulatorIsFalse_ThenRedirectsToSelectAnApprenticeshipUnit(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<ConfirmRegulatedShortCourseSubmitModel>> validator,
        [Greedy] ConfirmRegulatedShortCourseController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var learningType = LearningType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        validator.Setup(x => x.Validate(It.IsAny<ConfirmRegulatedShortCourseSubmitModel>())).Returns(new ValidationResult());

        // Act
        var response = sut.ConfirmRegulatedShortCourse(new ConfirmRegulatedShortCourseSubmitModel() { IsApprovedByRegulator = false, LearningType = learningType }, learningType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ConfirmRegulatedShortCourse);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void WhenIsCorrectShortCourseIsTrue_ThenRedirectsToConfirmSavedContactDetailsForShortCourse(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<ConfirmRegulatedShortCourseSubmitModel>> validator,
        [Greedy] ConfirmRegulatedShortCourseController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var learningType = LearningType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        validator.Setup(x => x.Validate(It.IsAny<ConfirmRegulatedShortCourseSubmitModel>())).Returns(new ValidationResult());

        // Act
        var response = sut.ConfirmRegulatedShortCourse(new ConfirmRegulatedShortCourseSubmitModel() { IsApprovedByRegulator = true, LearningType = learningType }, learningType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ConfirmSavedContactDetailsForShortCourse);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Delete(nameof(ShortCourseSessionModel)), Times.Never);
    }

    [Test, MoqAutoData]
    public void WhenSessionIsNull_ThenRedirectsToReviewYourDetails(
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Greedy] ConfirmRegulatedShortCourseController sut,
    ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var learningType = LearningType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(() => null);

        // Act
        var response = sut.ConfirmRegulatedShortCourse(new ConfirmRegulatedShortCourseSubmitModel() { LearningType = learningType }, learningType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Delete(nameof(ShortCourseSessionModel)), Times.Never);
    }

    [Test, MoqAutoData]
    public void WhenLatestProviderContactIsNullInSession_ThenRedirectsToAddShortCourseContactDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<ConfirmRegulatedShortCourseSubmitModel>> validator,
        [Greedy] ConfirmRegulatedShortCourseController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var learningType = LearningType.ApprenticeshipUnit;

        sessionModel.SavedProviderContactModel = null;

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        validator.Setup(x => x.Validate(It.IsAny<ConfirmRegulatedShortCourseSubmitModel>())).Returns(new ValidationResult());

        // Act
        var response = sut.ConfirmRegulatedShortCourse(new ConfirmRegulatedShortCourseSubmitModel() { IsApprovedByRegulator = true, LearningType = learningType }, learningType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.AddShortCourseContactDetails);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Delete(nameof(ShortCourseSessionModel)), Times.Never);
    }

    [Test, MoqAutoData]
    public void WhenEmailAddressAndPhoneNumberAreNullInSession_ThenRedirectsToAddShortCourseContactDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<ConfirmRegulatedShortCourseSubmitModel>> validator,
        [Greedy] ConfirmRegulatedShortCourseController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var learningType = LearningType.ApprenticeshipUnit;

        sessionModel.SavedProviderContactModel = new ProviderContactModel()
        {
            EmailAddress = null,
            PhoneNumber = null
        };

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        validator.Setup(x => x.Validate(It.IsAny<ConfirmRegulatedShortCourseSubmitModel>())).Returns(new ValidationResult());

        // Act
        var response = sut.ConfirmRegulatedShortCourse(new ConfirmRegulatedShortCourseSubmitModel() { IsApprovedByRegulator = true, LearningType = learningType }, learningType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.AddShortCourseContactDetails);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Delete(nameof(ShortCourseSessionModel)), Times.Never);
    }
}
