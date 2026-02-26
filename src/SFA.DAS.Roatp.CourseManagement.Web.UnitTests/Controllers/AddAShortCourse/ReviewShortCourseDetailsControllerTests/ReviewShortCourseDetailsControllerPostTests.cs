using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourse;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.ReviewShortCourseDetailsControllerTests;
public class ReviewShortCourseDetailsControllerPostTests
{
    [Test, MoqAutoData]
    public async Task ReviewShortCourseDetailsPost_ValidState_InvokesCommand(
        [Frozen] Mock<IValidator<ReviewShortCourseDetailsViewModel>> validator,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ReviewShortCourseDetailsController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        SaveShortCourseConfirmationViewModel viewModel = new SaveShortCourseConfirmationViewModel
        {
            CourseName = sessionModel.ShortCourseInformation.CourseName,
            ApprenticeshipType = apprenticeshipType
        };

        var expectedValueInTempData = JsonSerializer.Serialize(viewModel);

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        var tempDataMock = new Mock<ITempDataDictionary>();
        sut.TempData = tempDataMock.Object;

        validator.Setup(x => x.Validate(It.IsAny<ReviewShortCourseDetailsViewModel>())).Returns(new ValidationResult());

        // Act
        var result = await sut.ReviewShortCourseDetailsPost(apprenticeshipType);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<AddProviderCourseCommand>(c => c.Ukprn.ToString() == TestConstants.DefaultUkprn && c.LarsCode == sessionModel.LarsCode && c.UserId == TestConstants.DefaultUserId), It.IsAny<CancellationToken>()));
        result.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.SaveShortCourseConfirmation);
        tempDataMock.Verify(t => t.Add(TempDataKeys.SaveShortCourseBannerTempDataKey, expectedValueInTempData), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task ReviewShortCourseDetailsPost_InvalidState_RedirectsToGetReviewShortCourseDetails(
       [Frozen] Mock<IValidator<ReviewShortCourseDetailsViewModel>> validator,
       [Frozen] Mock<ISessionService> sessionServiceMock,
       [Greedy] ReviewShortCourseDetailsController sut,
       ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, "111") }, "mock"));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        var validationResult = new ValidationResult();

        validationResult.Errors.Add(new ValidationFailure("Field", "Error"));

        validator.Setup(x => x.Validate(It.IsAny<ReviewShortCourseDetailsViewModel>())).Returns(validationResult);

        sut.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user },
        };

        // Act
        var result = await sut.ReviewShortCourseDetailsPost(apprenticeshipType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewShortCourseDetails);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task ReviewShortCourseDetailsPost_SessionIsNull_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ReviewShortCourseDetailsController sut)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = await sut.ReviewShortCourseDetailsPost(apprenticeshipType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }
}
