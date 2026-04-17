using System.Text.Json;
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

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ConfirmAddProviderLocationControllerTests.ConfirmLocationTests;
public class WhenInAddStandardsJourney_AndGettingLocationConfirmation
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
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        StandardSessionModel sessionModel,
        AddressItem addressItem)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.ConfirmLocation(_learningType) as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        result.ViewName.Should().Be(ConfirmAddProviderLocationController.ViewPath);
        result.Model.Should().BeOfType<ConfirmAddProviderLocationViewModel>();
    }

    [Test, MoqAutoData]
    public async Task When_SessionIsValid_Then_ReturnExpectedAddress(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        StandardSessionModel sessionModel,
        AddressItem addressItem)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.ConfirmLocation(_learningType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.AddressLine1.Should().Be(addressItem.AddressLine1);
        model!.AddressLine2.Should().Be(addressItem.AddressLine2);
        model!.Town.Should().Be(addressItem.Town);
        model!.Postcode.Should().Be(addressItem.Postcode);
    }

    [Test, MoqAutoData]
    public async Task When_SessionIsValid_Then_RouteIsPostConfirmAddProviderLocation(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        StandardSessionModel sessionModel,
        AddressItem addressItem)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.ConfirmLocation(_learningType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.Route.Should().Be(RouteNames.PostConfirmAddProviderLocation);
    }

    [Test, MoqAutoData]
    public async Task When_SessionIsValid_Then_IsAddJourneyIsTrue(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        StandardSessionModel sessionModel,
        AddressItem addressItem)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.ConfirmLocation(_learningType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.IsAddJourney.Should().BeTrue();
    }

    [Test, MoqAutoData]
    public async Task When_SessionIsValid_Then_ReturnsExpectedDisplayHeader(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        StandardSessionModel sessionModel,
        AddressItem addressItem)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.ConfirmLocation(_learningType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model.DisplayHeader.Should().Be("Add a standard");
    }

    [Test, MoqAutoData]
    public async Task When_SessionIsValid_Then_ReturnsExpectedCancelLinkUrl(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        Mock<IUrlHelper> urlHelperMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        StandardSessionModel sessionModel,
        AddressItem addressItem,
        string cancelLinkUrl)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.CancelAddProviderLocation, cancelLinkUrl);

        // Act
        var result = await sut.ConfirmLocation(_learningType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.CancelLink.Should().Be(cancelLinkUrl);
    }

    [Test, MoqAutoData]
    public async Task When_SessionIsValid_Then_SubmitButtonTextIsConfirm(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        StandardSessionModel sessionModel,
        AddressItem addressItem)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.ConfirmLocation(_learningType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model.SubmitButtonText.Should().Be(ButtonText.Confirm);
    }

    [Test, MoqAutoData]
    public async Task When_SessionIsNull_Then_RedirectsToReviewYourDetail(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);

        // Act
        var result = await sut.ConfirmLocation(_learningType) as RedirectToRouteResult;

        // Assert
        result!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }

    [Test, MoqAutoData]
    public async Task When_LocationsAreAvailable_Then_RedirectsToGetNewStandardViewTrainingLocationOptions(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem,
        StandardSessionModel sessionModel,
        GetAllProviderLocationsQueryResult locationsResponse)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(locationsResponse);

        // Act
        var result = await sut.ConfirmLocation(_learningType) as RedirectToRouteResult;

        // Assert
        result!.RouteName.Should().Be(RouteNames.GetNewStandardViewTrainingLocationOptions);
    }

    [Test, MoqAutoData]
    public async Task When_LocationsAreAvailable_Then_VerifiesMediatorIsInvoked(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem,
        StandardSessionModel sessionModel,
        GetAllProviderLocationsQueryResult locationsResponse)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(locationsResponse);

        // Act
        await sut.ConfirmLocation(_learningType);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task When_AddressNotInTempData_Then_RedirectsToReviewYourDetails(
    Mock<ITempDataDictionary> tempDataMock,
    [Greedy] ConfirmAddProviderLocationController sut)
    {
        // Arrange
        object address = null;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        var result = await sut.ConfirmLocation(_learningType) as RedirectToRouteResult;

        // Assert
        result!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }
}
