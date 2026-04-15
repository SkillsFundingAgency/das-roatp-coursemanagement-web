using System.Collections.Generic;
using System.Linq;
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
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.CreateProviderLocation;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ConfirmAddProviderLocationControllerTests;
public class ConfirmLocationAddPostTests
{
    [Test, MoqAutoData]
    public void When_AddressMissingInTempData_Then_RedirectsToGetProviderLocations(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel model)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        object address = null;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        //Act
        var result = sut.ConfirmLocationAdd(model, apprenticeshipType).Result as RedirectToRouteResult;

        //Assert
        result.Should().NotBeNull();
        result.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        mediatorMock.Verify(m => m.Send(It.IsAny<CreateProviderLocationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    [MoqInlineAutoData(ApprenticeshipType.Apprenticeship)]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit)]
    public void When_ApprenticeshipTypeIsApprenticeshipOrApprenticeshipUnitAndModelIsInvalid_Then_ReturnsExpectedView(
    ApprenticeshipType apprenticeshipType,
    Mock<ITempDataDictionary> tempDataMock,
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] ConfirmAddProviderLocationController sut,
    ProviderLocationDetailsSubmitModel model,
    ShortCourseSessionModel shortCourseSessionModel,
    StandardSessionModel standardSessionModel,
    AddressItem addressItem)
    {
        // Arrange
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        if (apprenticeshipType == ApprenticeshipType.Apprenticeship)
        {
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);
        }

        if (apprenticeshipType == ApprenticeshipType.ApprenticeshipUnit)
        {
            sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(shortCourseSessionModel);
        }

        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = sut.ConfirmLocationAdd(model, apprenticeshipType).Result as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be(ConfirmAddProviderLocationController.ViewPath);
        var actualModel = (ConfirmAddProviderLocationViewModel)result.Model;
        actualModel!.LocationName.Should().Be(model.LocationName);
        actualModel!.Route.Should().Be(RouteNames.PostConfirmAddProviderLocation);
        actualModel!.IsAddJourney.Should().Be(true);
        mediatorMock.Verify(m => m.Send(It.IsAny<CreateProviderLocationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    [MoqInlineAutoData(ApprenticeshipType.Apprenticeship, "Add a standard")]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit, "Add an apprenticeship unit")]
    public void When_ApprenticeshipTypeIsApprenticeshipOrApprenticeshipUnitAndModelIsInvalid_Then_ReturnsExpectedDisplayHeader(
        ApprenticeshipType apprenticeshipType,
        string expectedDisplayHeader,
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel model,
        ShortCourseSessionModel shortCourseSessionModel,
        StandardSessionModel standardSessionModel,
        AddressItem addressItem)
    {
        // Arrange
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        if (apprenticeshipType == ApprenticeshipType.Apprenticeship)
        {
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);
        }

        if (apprenticeshipType == ApprenticeshipType.ApprenticeshipUnit)
        {
            sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(shortCourseSessionModel);
        }

        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = sut.ConfirmLocationAdd(model, apprenticeshipType).Result as ViewResult;

        // Assert
        var viewModel = result.Model as ConfirmAddProviderLocationViewModel;
        viewModel.DisplayHeader.Should().Be(expectedDisplayHeader);
    }

    [Test]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit, true, ButtonText.Confirm)]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit, false, ButtonText.Continue)]
    [MoqInlineAutoData(ApprenticeshipType.Apprenticeship, false, ButtonText.Confirm)]
    public void When_ApprenticeshipTypeIsApprenticeshipOrApprenticeshipUnitAndHasSeenSummaryPageIsTrueOrFalse_Then_ReturnsExpectedButtonText(
    ApprenticeshipType apprenticeshipType,
    bool hasSeenSummaryPage,
    string expectedSubmitButtonText,
    Mock<ITempDataDictionary> tempDataMock,
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] ConfirmAddProviderLocationController sut,
    ProviderLocationDetailsSubmitModel model,
    ShortCourseSessionModel shortCourseSessionModel,
    StandardSessionModel standardSessionModel,
    AddressItem addressItem)
    {
        // Arrange
        shortCourseSessionModel.HasSeenSummaryPage = hasSeenSummaryPage;
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        if (apprenticeshipType == ApprenticeshipType.Apprenticeship)
        {
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);
        }

        if (apprenticeshipType == ApprenticeshipType.ApprenticeshipUnit)
        {
            sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(shortCourseSessionModel);
        }

        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = sut.ConfirmLocationAdd(model, apprenticeshipType).Result as ViewResult;

        // Assert
        result.ViewName.Should().Be(ConfirmAddProviderLocationController.ViewPath);
        var actualModel = (ConfirmAddProviderLocationViewModel)result.Model;
        actualModel.SubmitButtonText.Should().Be(expectedSubmitButtonText);
    }

    [Test]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit, true, false)]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit, false, true)]
    [MoqInlineAutoData(ApprenticeshipType.Apprenticeship, false, true)]
    public void When_ApprenticeshipTypeIsApprenticeshipOrApprenticeshipUnitAndHasSeenSummaryPageIsTrueOrFalse_Then_ShowCancelButtonIsSetCorrectly(
    ApprenticeshipType apprenticeshipType,
    bool hasSeenSummaryPage,
    bool showCancelButton,
    Mock<ITempDataDictionary> tempDataMock,
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] ConfirmAddProviderLocationController sut,
    ProviderLocationDetailsSubmitModel model,
    ShortCourseSessionModel shortCourseSessionModel,
    StandardSessionModel standardSessionModel,
    AddressItem addressItem)
    {
        // Arrange
        shortCourseSessionModel.HasSeenSummaryPage = hasSeenSummaryPage;
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        if (apprenticeshipType == ApprenticeshipType.Apprenticeship)
        {
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);
        }

        if (apprenticeshipType == ApprenticeshipType.ApprenticeshipUnit)
        {
            sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(shortCourseSessionModel);
        }

        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = sut.ConfirmLocationAdd(model, apprenticeshipType).Result as ViewResult;

        // Assert
        result.ViewName.Should().Be(ConfirmAddProviderLocationController.ViewPath);
        var actualModel = (ConfirmAddProviderLocationViewModel)result.Model;
        actualModel.ShowCancelOption.Should().Be(showCancelButton);
    }

    [Test]
    [MoqInlineAutoData(ApprenticeshipType.Apprenticeship)]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit)]
    public void When_ApprenticeshipTypeIsApprenticeshipOrApprenticeshipUnitAndModelIsValid_Then_InvokesMediatorWithCreateCommandAndVerifyTempDataIsRemoved(
        ApprenticeshipType apprenticeshipType,
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel submitModel,
        StandardSessionModel standardSessionModel,
        AddressItem addressItem,
        GetAllProviderLocationsQueryResult queryResult)
    {
        // Arrange
        var shortCourseSessionModel = new ShortCourseSessionModel();
        shortCourseSessionModel.LocationOptions =
        [
            ShortCourseLocationOption.ProviderLocation
        ];
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        if (apprenticeshipType == ApprenticeshipType.Apprenticeship)
        {
            sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);
        }

        if (apprenticeshipType == ApprenticeshipType.ApprenticeshipUnit)
        {
            sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(shortCourseSessionModel);
        }

        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        var result = sut.ConfirmLocationAdd(submitModel, apprenticeshipType).Result as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        tempDataMock.Verify(t => t.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey));
        mediatorMock.Verify(m => m.Send(It.Is<CreateProviderLocationCommand>(c =>
            c.Ukprn.ToString() == TestConstants.DefaultUkprn &&
            c.UserId == TestConstants.DefaultUserId &&
            c.LocationName == submitModel.LocationName &&
            c.AddressLine1 == addressItem.AddressLine1 &&
            c.AddressLine2 == addressItem.AddressLine2 &&
            c.Town == addressItem.Town &&
            c.Postcode == addressItem.Postcode &&
            c.County == addressItem.County &&
            c.Latitude == addressItem.Latitude &&
            c.Longitude == addressItem.Longitude
        ), It.IsAny<CancellationToken>()));
    }

    [Test, MoqAutoData]
    public void When_ApprenticeshipTypeIsApprenticeshipUnitAndModelIsValid_Then_RedirectsToReviewShortCourseDetails(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel submitModel,
        AddressItem addressItem,
        GetAllProviderLocationsQueryResult queryResult)
    {
        // Arrange
        var sessionModel = new ShortCourseSessionModel();
        sessionModel.LocationOptions =
        [
            ShortCourseLocationOption.ProviderLocation
        ];
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        var result = sut.ConfirmLocationAdd(submitModel, apprenticeshipType).Result as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result.RouteName.Should().Be(RouteNames.ReviewShortCourseDetails);
    }

    [Test, MoqAutoData]
    public void When_ApprenticeshipTypeIsApprenticeshipAndModelIsValid_Then_RedirectsToGetNewStandardViewTrainingLocationOptions(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel submitModel,
        StandardSessionModel sessionModel,
        AddressItem addressItem,
        GetAllProviderLocationsQueryResult queryResult)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.Apprenticeship;
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);
        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(sessionModel);
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        var result = sut.ConfirmLocationAdd(submitModel, apprenticeshipType).Result as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result.RouteName.Should().Be(RouteNames.GetNewStandardViewTrainingLocationOptions);
    }

    [Test, MoqAutoData]
    public void When_ApprenticeshipTypeIsApprenticeshipUnitAndSessionContainsEmployerLocationOption_Then_SetsSessionAndRedirectsToConfirmNationalProviderDelivery(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel submitModel,
        AddressItem addressItem,
        GetAllProviderLocationsQueryResult queryResult)
    {
        // Arrange
        var sessionModel = new ShortCourseSessionModel();
        sessionModel.LocationOptions =
        [
            ShortCourseLocationOption.ProviderLocation,
            ShortCourseLocationOption.EmployerLocation,
        ];
        sessionModel.HasNationalDeliveryOption = null;
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        var result = sut.ConfirmLocationAdd(submitModel, apprenticeshipType).Result as RedirectToRouteResult;

        // Assert
        result.RouteName.Should().Be(RouteNames.ConfirmNationalDelivery);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Exactly(2));
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.TrainingVenues.FirstOrDefault().LocationName == queryResult.ProviderLocations.FirstOrDefault().LocationName && m.LocationsAvailable)), Times.Once);
    }

    [Test, MoqAutoData]
    public void When_ApprenticeshipTypeIsApprenticeshipUnitAndHasNationalDeliveryOptionIsFalseAndRegionsMissing_Then_SetsSessionAndRedirectsToSelectShortCourseRegions(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel submitModel,
        AddressItem addressItem,
        GetAllProviderLocationsQueryResult queryResult)
    {
        // Arrange
        var sessionModel = new ShortCourseSessionModel();
        sessionModel.LocationOptions =
        [
            ShortCourseLocationOption.ProviderLocation,
            ShortCourseLocationOption.EmployerLocation,
        ];
        sessionModel.HasNationalDeliveryOption = false;
        sessionModel.TrainingRegions = new List<TrainingRegionModel>();
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        var result = sut.ConfirmLocationAdd(submitModel, apprenticeshipType).Result as RedirectToRouteResult;

        // Assert
        result.RouteName.Should().Be(RouteNames.SelectShortCourseRegions);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Exactly(2));
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.TrainingVenues.FirstOrDefault().LocationName == queryResult.ProviderLocations.FirstOrDefault().LocationName && m.LocationsAvailable)), Times.Once);
    }

    [Test, MoqAutoData]
    public void When_AddressHasEmptyFieldsAndModelStateIsValid_Then_InvokesMediatorWithCreateCommand(
    Mock<ITempDataDictionary> tempDataMock,
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Greedy] ConfirmAddProviderLocationController sut,
    ProviderLocationDetailsSubmitModel submitModel,
    AddressItem addressItem)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        addressItem.AddressLine2 = null;
        addressItem.Town = null;
        addressItem.County = null;
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        var result = sut.ConfirmLocationAdd(submitModel, apprenticeshipType).Result as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result.RouteName.Should().Be(RouteNames.ReviewShortCourseDetails);
        tempDataMock.Verify(t => t.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey));
        mediatorMock.Verify(m => m.Send(It.Is<CreateProviderLocationCommand>(c =>
            c.Ukprn.ToString() == TestConstants.DefaultUkprn &&
            c.UserId == TestConstants.DefaultUserId &&
            c.LocationName == submitModel.LocationName &&
            c.AddressLine1 == addressItem.AddressLine1 &&
            c.AddressLine2 == string.Empty &&
            c.Town == string.Empty &&
            c.Postcode == addressItem.Postcode &&
            c.County == string.Empty &&
            c.Latitude == addressItem.Latitude &&
            c.Longitude == addressItem.Longitude
        ), It.IsAny<CancellationToken>()));
        mediatorMock.Verify(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test, MoqAutoData]
    public void When_LocationNameIsNotDistinct_Then_ReturnsViewResult(
    Mock<ITempDataDictionary> tempDataMock,
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] ConfirmAddProviderLocationController sut,
    ProviderLocationDetailsSubmitModel model,
    AddressItem addressItem,
    GetAllProviderLocationsQueryResult allLocations)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        allLocations.ProviderLocations.FirstOrDefault().LocationName = model.LocationName;
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(allLocations);
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        var result = sut.ConfirmLocationAdd(model, apprenticeshipType).Result as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be(ConfirmAddProviderLocationController.ViewPath);
        mediatorMock.Verify(m => m.Send(It.IsAny<CreateProviderLocationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        sut.ModelState.ErrorCount.Should().Be(1);
        sut.ModelState.Should().ContainKey("LocationName");
    }


    [Test]
    [MoqInlineAutoData(ApprenticeshipType.Apprenticeship)]
    [MoqInlineAutoData(ApprenticeshipType.ApprenticeshipUnit)]
    public async Task When_ApprenticeshipTypeIsApprenticeshipOrApprenticeshipUnitAndSessionIsNull_Then_RedirectsToReviewYourDetails(
        ApprenticeshipType apprenticeshipType,
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel model,
        AddressItem addressItem)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = await sut.ConfirmLocationAdd(model, apprenticeshipType) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }
}
