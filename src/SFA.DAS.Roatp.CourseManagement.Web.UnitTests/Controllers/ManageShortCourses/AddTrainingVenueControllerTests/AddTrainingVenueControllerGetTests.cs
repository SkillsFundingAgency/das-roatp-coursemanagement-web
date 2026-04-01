using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.AddTrainingVenueControllerTests;
public class AddTrainingVenueControllerGetTests
{
    [Test]
    [MoqInlineAutoData("", RouteNames.PostAddTrainingVenue, true)]
    [MoqInlineAutoData("test", RouteNames.PostAddTrainingVenueEditShortCourse, false)]
    public async Task LookupAddress_ReturnsExpectedView(
        string larsCode,
        string expectedPostRoute,
        bool expectedIsAddJourney,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] AddTrainingVenueController sut,
        ShortCourseSessionModel sessionModel,
        GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(() => new GetAllProviderLocationsQueryResult());

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var addressSearch = await sut.LookupAddress(apprenticeshipType, larsCode) as ViewResult;

        // Assert
        addressSearch.Should().NotBeNull();
        addressSearch.ViewName.Should().Be(AddTrainingVenueController.ViewPath);
        var model = addressSearch.Model as AddTrainingVenueViewModel;
        model.Route.Should().Be(expectedPostRoute);
        model.IsAddJourney.Should().Be(expectedIsAddJourney);
    }

    [Test]
    [MoqInlineAutoData("", true, "Confirm")]
    [MoqInlineAutoData("test", false, "Continue")]
    public async Task LookupAddress_HasSeenSummaryPageIsTrueOrFalse_ReturnsExpectedButtonText(
    string larsCode,
    bool hasSeenSummaryPage,
    string expectedSubmitButtonText,
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Frozen] Mock<ITempDataDictionary> tempDataMock,
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] AddTrainingVenueController sut,
    ShortCourseSessionModel sessionModel,
    GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sessionModel.HasSeenSummaryPage = hasSeenSummaryPage;

        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(() => new GetAllProviderLocationsQueryResult());

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var addressSearch = await sut.LookupAddress(apprenticeshipType, larsCode);

        // Assert
        var model = addressSearch.As<ViewResult>().Model as AddTrainingVenueViewModel;
        model.SubmitButtonText.Should().Be(expectedSubmitButtonText);
    }

    [Test, MoqAutoData]
    public async Task LookupAddress_IsAddJourney_SessionIsNull_RedirectsToReviewYourDetails(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] AddTrainingVenueController sut)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = await sut.LookupAddress(apprenticeshipType, "") as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task LookupAddress_IsAddJourney_LocationsAvailableIsTrueInSession_RedirectsToSelectShortCourseTrainingVenue(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] AddTrainingVenueController sut,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sessionModel.LocationsAvailable = true;

        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.LookupAddress(apprenticeshipType, "") as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.SelectShortCourseTrainingVenue);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task LookupAddress_ProviderCourseDoesNotExist_RedirectToPageNotFound(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] AddTrainingVenueController sut,
        ShortCourseSessionModel sessionModel,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(() => null);

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.LookupAddress(apprenticeshipType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
    }

    [Test, MoqAutoData]
    public async Task LookupAddress_ProviderLocationsExist_RedirectToEditShortCourseTrainingVenues(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] AddTrainingVenueController sut,
        ShortCourseSessionModel sessionModel,
        GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
        GetAllProviderLocationsQueryResult providerLocationsApiResponse,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(providerLocationsApiResponse);

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.LookupAddress(apprenticeshipType, larsCode) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.EditShortCourseTrainingVenues);
    }
}
