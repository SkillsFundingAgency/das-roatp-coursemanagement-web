using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.SelectShortCourseLocationOptionsControllerTests;
public class SelectShortCourseLocationOptionsControllerPostTests
{
    [Test, MoqAutoData]
    public async Task ConfirmSavedContactDetails_InvalidState_ReturnsView(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationOptionsController sut,
        ShortCourseSessionModel sessionModel,
        SelectShortCourseLocationOptionsSubmitModel submitModel)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;
        var expectedLocationOptions = Enum.GetValues<ShortCourseLocationOption>().ToList();
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        sut.ModelState.AddModelError("key", "message");

        // Act
        var response = await sut.SelectShortCourseLocation(submitModel, courseType);

        // Assert
        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as SelectShortCourseLocationOptionsViewModel;
        model.Should().NotBeNull();
        model!.LocationOptions.Select(x => x.LocationOption).Should().BeEquivalentTo(expectedLocationOptions);
        model!.CourseType.Should().Be(courseType);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.LocationOptions.FirstOrDefault() == submitModel.SelectedLocationOptions.FirstOrDefault() && m.HasOnlineDeliveryOption == submitModel.SelectedLocationOptions.Contains(ShortCourseLocationOption.Online))), Times.Never());
        mediatorMock.Verify(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Test, MoqAutoData]
    public async Task SelectShortCourseLocation_OnlineOptionIsSelected_SetsSessionCorrectlyAndRedirectsToSelectShortCourseLocation(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationOptionsController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        var submitModel = new SelectShortCourseLocationOptionsSubmitModel() { SelectedLocationOptions = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.Online } };

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.AddDefaultContextWithUser();

        // Act
        var response = await sut.SelectShortCourseLocation(submitModel, courseType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.SelectShortCourseLocationOption);
        sessionModel.HasOnlineDeliveryOption.Should().BeTrue();
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.LocationOptions.FirstOrDefault() == submitModel.SelectedLocationOptions.FirstOrDefault() && m.HasOnlineDeliveryOption == submitModel.SelectedLocationOptions.Contains(ShortCourseLocationOption.Online))), Times.Once());
        mediatorMock.Verify(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Test, MoqAutoData]
    public async Task SelectShortCourseLocation_OnlineOptionIsNotSelected_SetsSessionCorrectlyAndRedirectsToSelectShortCourseLocation(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationOptionsController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        var submitModel = new SelectShortCourseLocationOptionsSubmitModel() { SelectedLocationOptions = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.EmployerLocation } };

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.AddDefaultContextWithUser();

        // Act
        var response = await sut.SelectShortCourseLocation(submitModel, courseType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.SelectShortCourseLocationOption);
        sessionModel.HasOnlineDeliveryOption.Should().BeFalse();
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.LocationOptions.FirstOrDefault() == submitModel.SelectedLocationOptions.FirstOrDefault() && m.HasOnlineDeliveryOption == submitModel.SelectedLocationOptions.Contains(ShortCourseLocationOption.Online))), Times.Once());
        mediatorMock.Verify(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Never());
    }

    [Test, MoqAutoData]
    public async Task SelectShortCourseLocation_ProviderLocationOptionIsSelected_SetsSessionCorrectlyAndRedirectsToSelectShortCourseTrainingVenue(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationOptionsController sut,
        ShortCourseSessionModel sessionModel,
        GetAllProviderLocationsQueryResult queryResult)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        var submitModel = new SelectShortCourseLocationOptionsSubmitModel() { SelectedLocationOptions = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.ProviderLocation } };

        sessionModel.TrainingVenues = new List<TrainingVenueModel>();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sut.AddDefaultContextWithUser();

        // Act
        var response = await sut.SelectShortCourseLocation(submitModel, courseType);

        // Assert
        var redirectResult = response as RedirectToRouteResult;
        redirectResult!.RouteName.Should().Be(RouteNames.SelectShortCourseTrainingVenue);
        sessionModel.HasOnlineDeliveryOption.Should().BeFalse();
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.LocationOptions.FirstOrDefault() == submitModel.SelectedLocationOptions.FirstOrDefault() && m.HasOnlineDeliveryOption == submitModel.SelectedLocationOptions.Contains(ShortCourseLocationOption.Online) && m.TrainingVenues.FirstOrDefault().ProviderLocationId == queryResult.ProviderLocations.FirstOrDefault().NavigationId)), Times.Once);
        mediatorMock.Verify(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test, MoqAutoData]
    public async Task SelectShortCourseLocation_SessionIsNull_RedirectsToReviewYourDetails(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseLocationOptionsController sut)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = await sut.SelectShortCourseLocation(new SelectShortCourseLocationOptionsSubmitModel(), courseType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.IsAny<ShortCourseSessionModel>()), Times.Never);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        mediatorMock.Verify(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Never());
    }
}
