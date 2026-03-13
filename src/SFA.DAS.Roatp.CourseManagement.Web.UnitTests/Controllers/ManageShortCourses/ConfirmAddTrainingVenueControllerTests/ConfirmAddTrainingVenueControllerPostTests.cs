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
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.ConfirmAddTrainingVenueControllerTests;
public class ConfirmAddTrainingVenueControllerPostTests
{
    [Test, MoqAutoData]
    public void ConfirmVenue_AddressMissingInTempData_RedirectsToGetProviderLocations(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        ConfirmAddTrainingVenueSubmitModel model)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        var larsCode = "test";
        object address = null;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        //Act
        var result = sut.ConfirmVenue(model, apprenticeshipType, larsCode).Result as RedirectToRouteResult;

        //Assert
        result.Should().NotBeNull();
        result.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        mediatorMock.Verify(m => m.Send(It.IsAny<CreateProviderLocationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    [MoqInlineAutoData("")]
    [MoqInlineAutoData("test")]
    public void ConfirmVenue_ModelStateIsInvalid_ReturnsViewResult(
        string larsCode,
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        ConfirmAddTrainingVenueSubmitModel model,
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = sut.ConfirmVenue(model, apprenticeshipType, larsCode).Result as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be(ConfirmAddTrainingVenueController.ViewPath);
        var actualModel = (ConfirmAddTrainingVenueViewModel)result.Model;
        actualModel!.LocationName.Should().Be(model.LocationName);
        mediatorMock.Verify(m => m.Send(It.IsAny<CreateProviderLocationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    [MoqInlineAutoData("", true, ButtonText.Confirm)]
    [MoqInlineAutoData("test", false, ButtonText.Continue)]
    public void ConfirmVenue_HasSeenSummaryPageIsTrueOrFalse_ReturnsExpectedButtonText(
    string larsCode,
    bool hasSeenSummaryPage,
    string expectedSubmitButtonText,
    Mock<ITempDataDictionary> tempDataMock,
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] ConfirmAddTrainingVenueController sut,
    ConfirmAddTrainingVenueSubmitModel model,
    ShortCourseSessionModel sessionModel,
    AddressItem addressItem)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        sessionModel.HasSeenSummaryPage = hasSeenSummaryPage;
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = sut.ConfirmVenue(model, apprenticeshipType, larsCode).Result as ViewResult;

        // Assert
        result.ViewName.Should().Be(ConfirmAddTrainingVenueController.ViewPath);
        var actualModel = (ConfirmAddTrainingVenueViewModel)result.Model;
        actualModel.SubmitButtonText.Should().Be(expectedSubmitButtonText);
    }

    [Test]
    [MoqInlineAutoData("", true, false)]
    [MoqInlineAutoData("test", false, true)]
    [MoqInlineAutoData("", false, true)]
    [MoqInlineAutoData("test", true, true)]
    public void ConfirmVenue_HasSeenSummaryPageIsTrueOrFalse_ShowCancelButtonSetsCorrectly(
    string larsCode,
    bool hasSeenSummaryPage,
    bool showCancelButton,
    Mock<ITempDataDictionary> tempDataMock,
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] ConfirmAddTrainingVenueController sut,
    ConfirmAddTrainingVenueSubmitModel model,
    ShortCourseSessionModel sessionModel,
    AddressItem addressItem)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        sessionModel.HasSeenSummaryPage = hasSeenSummaryPage;
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = sut.ConfirmVenue(model, apprenticeshipType, larsCode).Result as ViewResult;

        // Assert
        result.ViewName.Should().Be(ConfirmAddTrainingVenueController.ViewPath);
        var actualModel = (ConfirmAddTrainingVenueViewModel)result.Model;
        actualModel.ShowCancelOption.Should().Be(showCancelButton);
    }

    [Test, MoqAutoData]
    public void ConfirmVenue_ModelStateIsValid_InvokesMediatorWithCreateCommandndRedirectsToGetConfirmAddTrainingVenue(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        ConfirmAddTrainingVenueSubmitModel submitModel,
        AddressItem addressItem,
        GetAllProviderLocationsQueryResult queryResult)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        var result = sut.ConfirmVenue(submitModel, apprenticeshipType, "test").Result as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result.RouteName.Should().Be(RouteNames.GetConfirmAddTrainingVenue);
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
        mediatorMock.Verify(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Once);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Never);
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.TrainingVenues.FirstOrDefault().LocationName == queryResult.ProviderLocations.FirstOrDefault().LocationName && m.LocationsAvailable)), Times.Never);
    }

    [Test, MoqAutoData]
    public void ConfirmVenue_AddressHasEmptyFields_ModelStateIsValid_InvokesMediatorWithCreateCommand(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        ConfirmAddTrainingVenueSubmitModel submitModel,
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

        // Act
        var result = sut.ConfirmVenue(submitModel, apprenticeshipType, larsCode).Result as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result.RouteName.Should().Be(RouteNames.GetConfirmAddTrainingVenue);
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
        mediatorMock.Verify(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public void ConfirmVenue_LocationNameIsNotDistinct_ReturnsViewResult(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        ConfirmAddTrainingVenueSubmitModel model,
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
        var result = sut.ConfirmVenue(model, apprenticeshipType, larsCode).Result as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be(ConfirmAddTrainingVenueController.ViewPath);
        mediatorMock.Verify(m => m.Send(It.IsAny<CreateProviderLocationCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        sut.ModelState.ErrorCount.Should().Be(1);
        sut.ModelState.Should().ContainKey("LocationName");
    }
}
