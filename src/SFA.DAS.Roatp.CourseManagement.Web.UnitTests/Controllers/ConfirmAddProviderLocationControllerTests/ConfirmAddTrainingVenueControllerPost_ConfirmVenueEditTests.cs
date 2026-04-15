using System.Linq;
using System.Text.Json;
using System.Threading;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Commands.CreateProviderLocation;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAllProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourseLocation;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ConfirmAddProviderLocationControllerTests;
public class ConfirmAddTrainingVenueControllerPost_ConfirmVenueEditTests
{
    [Test, MoqAutoData]
    public void ConfirmVenueEdit_AddressMissingInTempData_RedirectsToEditShortCourseTrainingVenues(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel model)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        var larsCode = "test";
        object address = null;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        //Act
        var result = sut.ConfirmLocationEdit(model, apprenticeshipType, larsCode).Result as RedirectToRouteResult;

        //Assert
        result.Should().NotBeNull();
        result.RouteName.Should().Be(RouteNames.EditShortCourseTrainingVenues);
        mediatorMock.Verify(m => m.Send(It.IsAny<CreateProviderLocationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, MoqAutoData]
    public void ConfirmVenueEdit_ModelStateIsInvalid_ReturnsViewResult(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel model,
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        var larsCode = "test";
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = sut.ConfirmLocationEdit(model, apprenticeshipType, larsCode).Result as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be(ConfirmAddProviderLocationController.ViewPath);
        var actualModel = (ConfirmAddProviderLocationViewModel)result.Model;
        actualModel!.LocationName.Should().Be(model.LocationName);
        actualModel!.Route.Should().Be(RouteNames.PostConfirmAddTrainingVenueEditShortCourse);
        actualModel!.IsAddJourney.Should().Be(false);
        actualModel!.SubmitButtonText.Should().Be(ButtonText.Confirm);
        actualModel!.ShowCancelOption.Should().BeFalse();
        mediatorMock.Verify(m => m.Send(It.IsAny<CreateProviderLocationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    [MoqInlineAutoData(null, RouteNames.ManageShortCourseDetails)]
    [MoqInlineAutoData("EmployerLocation", RouteNames.EditShortCourseNationalDelivery)]
    public void ConfirmVenueEdit_EmployerLocationVariationInSession_InvokesMediatorCorrectlyAndRedirectsToCorrectRoute(
        string locationInSession,
        string expectedRedirectRoute,
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel submitModel,
        AddressItem addressItem,
        GetAllProviderLocationsQueryResult queryResult,
        GetAllProviderLocationsQueryResult providerLocationsApiResponse,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);
        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(providerLocationsApiResponse);
        sessionServiceMock.Setup(s => s.Get(SessionKeys.SelectedShortCourseLocationOption)).Returns(locationInSession);
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        var result = sut.ConfirmLocationEdit(submitModel, apprenticeshipType, larsCode).Result as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result.RouteName.Should().Be(expectedRedirectRoute);
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
        mediatorMock.Verify(m => m.Send(It.Is<AddProviderCourseLocationCommand>(q =>
        q.Ukprn.ToString() == TestConstants.DefaultUkprn &&
        q.UserId == TestConstants.DefaultUserId &&
        q.LarsCode == larsCode &&
        q.LocationNavigationId == providerLocationsApiResponse.ProviderLocations.FirstOrDefault().NavigationId &&
        q.HasDayReleaseDeliveryOption == false &&
        q.HasBlockReleaseDeliveryOption == false), It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Exactly(2));
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Never);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.TrainingVenues.FirstOrDefault().LocationName == queryResult.ProviderLocations.FirstOrDefault().LocationName && m.LocationsAvailable)), Times.Never);
    }

    [Test, MoqAutoData]
    public void ConfirmVenueEdit_AddressHasEmptyFields_InvokesMediatorWithCreateCommand(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel submitModel,
        AddressItem addressItem,
        GetAllProviderLocationsQueryResult providerLocationsApiResponse)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        var larsCode = "test";
        addressItem.AddressLine2 = null;
        addressItem.Town = null;
        addressItem.County = null;
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));
        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(providerLocationsApiResponse);

        // Act
        var result = sut.ConfirmLocationEdit(submitModel, apprenticeshipType, larsCode).Result as RedirectToRouteResult;

        // Assert
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
        mediatorMock.Verify(m => m.Send(It.Is<AddProviderCourseLocationCommand>(q =>
        q.Ukprn.ToString() == TestConstants.DefaultUkprn &&
        q.UserId == TestConstants.DefaultUserId &&
        q.LarsCode == larsCode &&
        q.LocationNavigationId == providerLocationsApiResponse.ProviderLocations.FirstOrDefault().NavigationId &&
        q.HasDayReleaseDeliveryOption == false &&
        q.HasBlockReleaseDeliveryOption == false), It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test, MoqAutoData]
    public void ConfirmVenueEdit_LocationNameIsNotDistinct_ReturnsViewResult(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel model,
        AddressItem addressItem,
        GetAllProviderLocationsQueryResult allLocations)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        var larsCode = "test";
        allLocations.ProviderLocations.FirstOrDefault().LocationName = model.LocationName;
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(allLocations);
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        var result = sut.ConfirmLocationEdit(model, apprenticeshipType, larsCode).Result as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be(ConfirmAddProviderLocationController.ViewPath);
        mediatorMock.Verify(m => m.Send(It.IsAny<CreateProviderLocationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        sut.ModelState.ErrorCount.Should().Be(1);
        sut.ModelState.Should().ContainKey("LocationName");
    }

    [Test, MoqAutoData]
    public void ConfirmVenueEdit_AddedProviderLocationReturnsNull_RedirectsToEditShortCourseTrainingVenues(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel submitModel,
        AddressItem addressItem)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        var larsCode = "test";
        addressItem.AddressLine2 = null;
        addressItem.Town = null;
        addressItem.County = null;
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));
        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(() => new GetAllProviderLocationsQueryResult());

        // Act
        var result = sut.ConfirmLocationEdit(submitModel, apprenticeshipType, larsCode).Result as RedirectToRouteResult;

        // Assert
        result.RouteName.Should().Be(RouteNames.EditShortCourseTrainingVenues);
    }
}
