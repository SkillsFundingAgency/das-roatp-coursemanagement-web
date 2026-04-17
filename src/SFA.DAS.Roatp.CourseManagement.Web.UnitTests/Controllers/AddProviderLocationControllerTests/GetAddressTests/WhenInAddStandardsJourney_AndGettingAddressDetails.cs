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
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddProviderLocationControllerTests.GetAddressTests;
public class WhenInAddStandardsJourney_AndGettingAddressDetails
{
    private ApprenticeshipType _learningType;

    [SetUp]
    public void Before_Each_Test()
    {
        _learningType = ApprenticeshipType.Apprenticeship;
    }

    [Test, MoqAutoData]
    public async Task When_SessionIsValid_Then_ReturnsExpectedViewModel(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] AddProviderLocationController sut,
        StandardSessionModel sessionModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.GetAddress(_learningType) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be(AddProviderLocationController.ViewPath);
        result.Model.Should().BeOfType<AddProviderLocationViewModel>();
    }

    [Test, MoqAutoData]
    public async Task When_SessionIsValid_Then_RouteIsPostAddProviderLocation(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] AddProviderLocationController sut,
        StandardSessionModel sessionModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.GetAddress(_learningType) as ViewResult;

        // Assert
        var model = result.Model as AddProviderLocationViewModel;
        model.Route.Should().Be(RouteNames.PostAddProviderLocation);
    }

    [Test, MoqAutoData]
    public async Task When_SessionIsValid_Then_IsAddJourneyIsTrue(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] AddProviderLocationController sut,
        StandardSessionModel sessionModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.GetAddress(_learningType) as ViewResult;

        // Assert
        var model = result.Model as AddProviderLocationViewModel;
        model.IsAddJourney.Should().BeTrue();
    }

    [Test, MoqAutoData]
    public async Task When_SessionIsValid_Then_CorrectDisplayHeaderIsReturned(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] AddProviderLocationController sut,
        StandardSessionModel sessionModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.GetAddress(_learningType) as ViewResult;

        // Assert
        var model = result.Model as AddProviderLocationViewModel;
        model.DisplayHeader.Should().Be("Add a standard");
    }

    [Test, MoqAutoData]
    public async Task When_SessionIsValid_Then_SubmitButtonTextIsContinue(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] AddProviderLocationController sut,
        StandardSessionModel sessionModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.GetAddress(_learningType) as ViewResult;

        // Assert
        var model = result.Model as AddProviderLocationViewModel;
        model.SubmitButtonText.Should().Be(ButtonText.Continue);
    }

    [Test, MoqAutoData]
    public async Task When_SessionIsNull_Then_RedirectsToReviewYourDetails(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] AddProviderLocationController sut)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);

        // Act
        var result = await sut.GetAddress(_learningType) as RedirectToRouteResult;

        // Assert
        result!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }

    [Test, MoqAutoData]
    public async Task When_LocationsAreAvailable_Then_RedirectsToSelectShortCourseTrainingVenue(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] AddProviderLocationController sut,
        StandardSessionModel sessionModel,
        GetAllProviderLocationsQueryResult locationsResponse)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(locationsResponse);

        // Act
        var result = await sut.GetAddress(_learningType) as RedirectToRouteResult;

        // Assert
        result!.RouteName.Should().Be(RouteNames.GetNewStandardViewTrainingLocationOptions);
    }

    [Test, MoqAutoData]
    public async Task When_LocationsAreAvailable_Then_VerifiesMediatorIsInvoked(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] AddProviderLocationController sut,
        StandardSessionModel sessionModel,
        GetAllProviderLocationsQueryResult locationsResponse)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(locationsResponse);

        // Act
        await sut.GetAddress(_learningType);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task When_SessionIsValid_Then_VerifyTempDataIsRemoved(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] AddProviderLocationController sut,
        StandardSessionModel sessionModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

        // Act
        await sut.GetAddress(_learningType);

        // Assert
        tempDataMock.Verify(t => t.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey));
    }
}
