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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.SelectShortCourseTrainingVenuesControllerTests;
public class SelectShortCourseTrainingVenuesControllerGetTests
{
    [Test, MoqAutoData]
    public async Task SelectShortCourseTrainingVenue_SessionIsValid_ReturnsView(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] SelectShortCourseTrainingVenuesController sut,
        ShortCourseSessionModel sessionModel,
        GetAllProviderLocationsQueryResult queryResult
    )
    {
        // Arrange
        var courseType = CourseType.ShortCourse;

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        // Act
        var result = await sut.SelectShortCourseTrainingVenue(courseType);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult!.Model as SelectShortCourseTrainingVenuesViewModel;
        model!.TrainingVenues.Should().BeEquivalentTo(sessionModel.TrainingVenues);
        model.CourseType.Should().Be(courseType);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.TrainingVenues.FirstOrDefault().ProviderLocationId == queryResult.ProviderLocations.FirstOrDefault().NavigationId && m.LocationsAvailable)), Times.Once);
        mediatorMock.Verify(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test, MoqAutoData]
    public async Task SelectShortCourseTrainingVenue_SessionIsNull_RedirectsToReviewYourDetails(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseTrainingVenuesController sut,
        GetAllProviderLocationsQueryResult queryResult)
    {
        // Arrange
        var courseType = CourseType.ShortCourse;

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = await sut.SelectShortCourseTrainingVenue(courseType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.TrainingVenues.FirstOrDefault().ProviderLocationId == queryResult.ProviderLocations.FirstOrDefault().NavigationId && m.LocationsAvailable)), Times.Never);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }

    [Test, MoqAutoData]
    public async Task SelectShortCourseTrainingVenue_AtProviderLocationNotSelected_RedirectsToReviewYourDetails(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseTrainingVenuesController sut,
        ShortCourseSessionModel sessionModel,
        GetAllProviderLocationsQueryResult queryResult
    )
    {
        // Arrange
        var courseType = CourseType.ShortCourse;

        sessionModel.LocationOptions = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.Online };

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.SelectShortCourseTrainingVenue(courseType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.TrainingVenues.FirstOrDefault().ProviderLocationId == queryResult.ProviderLocations.FirstOrDefault().NavigationId && m.LocationsAvailable)), Times.Never);
        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }

    [Test, MoqAutoData]
    public async Task SelectShortCourseTrainingVenue_LocationsAvailableIsFalse_RedirectsToGetAddTrainingVenue(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectShortCourseTrainingVenuesController sut,
        ShortCourseSessionModel sessionModel

    )
    {
        // Arrange
        var courseType = CourseType.ShortCourse;

        GetAllProviderLocationsQueryResult queryResult = new();

        sessionModel.LocationOptions = new List<ShortCourseLocationOption>() { ShortCourseLocationOption.ProviderLocation };

        sessionModel.TrainingVenues = new List<TrainingVenueModel>();

        sessionModel.LocationsAvailable = false;

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.SelectShortCourseTrainingVenue(courseType);

        // Assert
        var redirectResult = result as RedirectToRouteResult;
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.TrainingVenues.SequenceEqual(new List<TrainingVenueModel>()) && !m.LocationsAvailable)), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.GetAddTrainingVenue);
    }
}
