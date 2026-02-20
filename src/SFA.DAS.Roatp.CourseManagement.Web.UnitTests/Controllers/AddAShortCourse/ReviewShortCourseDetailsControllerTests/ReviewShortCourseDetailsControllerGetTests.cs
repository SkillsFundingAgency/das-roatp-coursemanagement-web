using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Security.Claims;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.ReviewShortCourseDetailsControllerTests;
public class ReviewShortCourseDetailsControllerGetTests
{
    [Test, MoqAutoData]
    public void ReviewShortCourseDetails_SessionIsValid_ReturnsView(
        [Frozen] Mock<IValidator<ReviewShortCourseDetailsViewModel>> validator,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ReviewShortCourseDetailsController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, "111") }, "mock"));

        string cancelLink = Guid.NewGuid().ToString();
        string contactDetailsChangeLink = Guid.NewGuid().ToString();
        string regionsChangeLink = Guid.NewGuid().ToString();
        string trainingVenueChangeLink = Guid.NewGuid().ToString();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        validator.Setup(x => x.Validate(It.IsAny<ReviewShortCourseDetailsViewModel>())).Returns(new ValidationResult());

        sut.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user },
        };

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ReviewYourDetails, cancelLink)
            .AddUrlForRoute(RouteNames.AddShortCourseContactDetails, contactDetailsChangeLink)
            .AddUrlForRoute(RouteNames.SelectShortCourseRegions, regionsChangeLink)
            .AddUrlForRoute(RouteNames.SelectShortCourseTrainingVenue, trainingVenueChangeLink);

        // Act
        var result = sut.ReviewShortCourseDetails(apprenticeshipType);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult!.Model as ReviewShortCourseDetailsViewModel;
        model!.Should().NotBeNull();
        model.ApprenticeshipType.Should().Be(apprenticeshipType);
        model.CancelLink.Should().Be(cancelLink);
        model.ContactDetailsChangeLink.Should().Be(contactDetailsChangeLink);
        model.TrainingRegionsChangeLink.Should().Be(regionsChangeLink);
        model.TrainingVenuesChangeLink.Should().Be(trainingVenueChangeLink);
        sut.ModelState.ErrorCount.Should().Be(0);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.HasSeenSummaryPage)), Times.Once());
    }

    [Test, MoqAutoData]
    public void ReviewShortCourseDetails_InvalidState_ReturnsViewWithErrors(
        [Frozen] Mock<IValidator<ReviewShortCourseDetailsViewModel>> validator,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ReviewShortCourseDetailsController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, "111") }, "mock"));

        string cancelLink = Guid.NewGuid().ToString();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        var validationResult = new ValidationResult();

        validationResult.Errors.Add(new ValidationFailure("Field", "Error"));

        validator.Setup(x => x.Validate(It.IsAny<ReviewShortCourseDetailsViewModel>())).Returns(validationResult);

        sut.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user },
        };

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ReviewYourDetails, cancelLink);

        // Act
        var result = sut.ReviewShortCourseDetails(apprenticeshipType);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult!.Model as ReviewShortCourseDetailsViewModel;
        model!.Should().NotBeNull();
        model.ApprenticeshipType.Should().Be(apprenticeshipType);
        sut.ModelState.ErrorCount.Should().Be(1);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.HasSeenSummaryPage)), Times.Once());
    }

    [Test, MoqAutoData]
    public void ReviewShortCourseDetails_SessionIsNull_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ReviewShortCourseDetailsController sut)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = sut.ReviewShortCourseDetails(apprenticeshipType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.HasSeenSummaryPage)), Times.Never());
    }
}
