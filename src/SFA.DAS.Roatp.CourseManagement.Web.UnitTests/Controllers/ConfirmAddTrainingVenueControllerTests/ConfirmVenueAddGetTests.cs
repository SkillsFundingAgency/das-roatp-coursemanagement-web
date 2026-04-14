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
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ConfirmAddTrainingVenueControllerTests;
public class ConfirmVenueAddGetTests
{
    [Test]
    [MoqInlineAutoData(ApprenticeshipType.Apprenticeship)]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit)]
    public async Task When_ApprenticeshipTypeIsApprenticeshipOrApprenticeshipUnitAndSessionIsValid_Then_ReturnsExpectedView(
        ApprenticeshipType apprenticeshipType,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        Mock<IUrlHelper> urlHelperMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        ShortCourseSessionModel shortCourseSessionModel,
        StandardSessionModel standardSessionModel,
        AddressItem addressItem,
        string cancelLinkUrl)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        if (apprenticeshipType == ApprenticeshipType.Apprenticeship)
        {
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);
        }

        if (apprenticeshipType == ApprenticeshipType.ApprenticeshipUnit)
        {
            sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(shortCourseSessionModel);
        }

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.CancelAddTrainingVenue, cancelLinkUrl);

        // Act
        var result = await sut.ConfirmVenueAdd(apprenticeshipType) as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        result.ViewName.Should().Be(ConfirmAddTrainingVenueController.ViewPath);
        var model = result.Model as ConfirmAddTrainingVenueViewModel;
        model!.AddressLine1.Should().Be(addressItem.AddressLine1);
        model!.CancelLink.Should().Be(cancelLinkUrl);
        model!.Route.Should().Be(RouteNames.PostConfirmAddTrainingVenue);
        model!.IsAddJourney.Should().Be(true);
    }

    [Test]
    [MoqInlineAutoData(ApprenticeshipType.Apprenticeship, "Add a standard")]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit, "Add an apprenticeship unit")]
    public async Task When_ApprenticeshipTypeIsApprenticeshipOrApprenticeshipUnitAndSessionIsValid_Then_ReturnsExpectedDisplayHeader(
        ApprenticeshipType apprenticeshipType,
        string expectedDisplayHeader,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        ShortCourseSessionModel shortCourseSessionModel,
        StandardSessionModel standardSessionModel,
        AddressItem addressItem,
        string cancelLinkUrl)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        if (apprenticeshipType == ApprenticeshipType.Apprenticeship)
        {
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);
        }

        if (apprenticeshipType == ApprenticeshipType.ApprenticeshipUnit)
        {
            sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(shortCourseSessionModel);
        }

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.CancelAddTrainingVenue, cancelLinkUrl);

        // Act
        var result = await sut.ConfirmVenueAdd(apprenticeshipType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddTrainingVenueViewModel;
        model.DisplayHeader.Should().Be(expectedDisplayHeader);
    }

    [Test]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit, true, ButtonText.Confirm)]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit, false, ButtonText.Continue)]
    [MoqInlineAutoData(ApprenticeshipType.Apprenticeship, false, ButtonText.Confirm)]
    public async Task When_ApprenticeshipTypeIsApprenticeshipOrApprenticeshipUnitAndHasSeenSummaryPageIsTrueOrFalse_Then_ReturnsExpectedButtonText(
    ApprenticeshipType apprenticeshipType,
    bool hasSeenSummaryPage,
    string expectedSubmitButtonText,
    [Frozen] Mock<ISessionService> sessionServiceMock,
    Mock<ITempDataDictionary> tempDataMock,
    Mock<IUrlHelper> urlHelperMock,
    [Greedy] ConfirmAddTrainingVenueController sut,
    ShortCourseSessionModel shortCourseSessionModel,
    StandardSessionModel standardSessionModel,
    AddressItem addressItem,
    string cancelLinkUrl)
    {
        // Arrange
        shortCourseSessionModel.HasSeenSummaryPage = hasSeenSummaryPage;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        if (apprenticeshipType == ApprenticeshipType.Apprenticeship)
        {
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);
        }

        if (apprenticeshipType == ApprenticeshipType.ApprenticeshipUnit)
        {
            sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(shortCourseSessionModel);
        }

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.CancelAddTrainingVenue, cancelLinkUrl);

        // Act
        var result = await sut.ConfirmVenueAdd(apprenticeshipType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddTrainingVenueViewModel;
        model.SubmitButtonText.Should().Be(expectedSubmitButtonText);
    }

    [Test]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit, true, false)]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit, false, true)]
    [MoqInlineAutoData(ApprenticeshipType.Apprenticeship, false, true)]
    public async Task When_ApprenticeshipTypeIsApprenticeshipOrApprenticeshipUnitAndHasSeenSummaryPageIsTrueOrFalse_Then_ShowCancelButtonIsSetCorrectly(
        ApprenticeshipType apprenticeshipType,
        bool hasSeenSummaryPage,
        bool showCancelButton,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        Mock<IUrlHelper> urlHelperMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        ShortCourseSessionModel shortCourseSessionModel,
        StandardSessionModel standardSessionModel,
        AddressItem addressItem,
        string cancelLinkUrl)
    {
        // Arrange
        shortCourseSessionModel.HasSeenSummaryPage = hasSeenSummaryPage;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        if (apprenticeshipType == ApprenticeshipType.Apprenticeship)
        {
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);
        }

        if (apprenticeshipType == ApprenticeshipType.ApprenticeshipUnit)
        {
            sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(shortCourseSessionModel);
        }

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.CancelAddTrainingVenue, cancelLinkUrl);

        // Act
        var result = await sut.ConfirmVenueAdd(apprenticeshipType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddTrainingVenueViewModel;
        model.ShowCancelOption.Should().Be(showCancelButton);
    }

    [Test]
    [MoqInlineAutoData(ApprenticeshipType.Apprenticeship)]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit)]
    public async Task When_ApprenticeshipTypeIsApprenticeshipOrApprenticeshipUnitAndSessionIsNull_Then_RedirectsToReviewYourDetails(
        ApprenticeshipType apprenticeshipType,
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        AddressItem addressItem)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        if (apprenticeshipType == ApprenticeshipType.Apprenticeship)
        {
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);
        }

        if (apprenticeshipType == ApprenticeshipType.ApprenticeshipUnit)
        {
            sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);
        }

        // Act
        var result = await sut.ConfirmVenueAdd(apprenticeshipType) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }

    [Test, MoqAutoData]
    public async Task When_ApprenticeshipTypeIsApprenticeshipUnitAndLocationsAvailableIsTrueInSession_Then_RedirectsToSelectShortCourseTrainingVenue(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        AddressItem addressItem,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        sessionModel.LocationsAvailable = true;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.ConfirmVenueAdd(apprenticeshipType) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.SelectShortCourseTrainingVenue);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task When_ApprenticeshipTypeIsApprenticeshipAndLocationsAreAvailable_Then_RedirectsToGetNewStandardViewTrainingLocationOptions(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        AddressItem addressItem,
        StandardSessionModel sessionModel,
        GetAllProviderLocationsQueryResult locationsResponse)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.Apprenticeship;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(locationsResponse);

        // Act
        var result = await sut.ConfirmVenueAdd(apprenticeshipType) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.GetNewStandardViewTrainingLocationOptions);
    }

    [Test, MoqAutoData]
    public async Task When_ApprenticeshipTypeIsApprenticeshipAndLocationsAreAvailable_Then_VerifiesMediatorIsInvoked(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        AddressItem addressItem,
        StandardSessionModel sessionModel,
        GetAllProviderLocationsQueryResult locationsResponse)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.Apprenticeship;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);

        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(locationsResponse);

        // Act
        await sut.ConfirmVenueAdd(apprenticeshipType);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task When_AddressNotInTempData_Then_RedirectsToReviewYourDetails(
    Mock<ITempDataDictionary> tempDataMock,
    [Greedy] ConfirmAddTrainingVenueController sut)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        object address = null;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        var result = await sut.ConfirmVenueAdd(apprenticeshipType) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }
}
