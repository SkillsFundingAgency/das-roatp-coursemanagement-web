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
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ConfirmAddProviderLocationControllerTests.ConfirmLocationTests;
public class WhenInEditStandardsJourney_AndPostingLocationConfirmation
{
    private ApprenticeshipType _learningType;

    [SetUp]
    public void Before_Each_Test()
    {
        _learningType = ApprenticeshipType.Apprenticeship;
    }

    [Test, MoqAutoData]
    public void When_ModelStateIsInvalid_Then_ReturnsExpectedViewModel(
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem,
        ProviderLocationDetailsSubmitModel submitModel,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));
        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = sut.ConfirmLocation(submitModel, _learningType, larsCode).Result as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        result.ViewName.Should().Be(ConfirmAddProviderLocationController.ViewPath);
        result.Model.Should().BeOfType<ConfirmAddProviderLocationViewModel>();
    }

    [Test, MoqAutoData]
    public void When_ModelStateIsInvalid_Then_ReturnExpectedAddress(
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem,
        ProviderLocationDetailsSubmitModel submitModel,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));
        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = sut.ConfirmLocation(submitModel, _learningType, larsCode).Result as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.AddressLine1.Should().Be(addressItem.AddressLine1);
        model!.AddressLine2.Should().Be(addressItem.AddressLine2);
        model!.Town.Should().Be(addressItem.Town);
        model!.Postcode.Should().Be(addressItem.Postcode);
    }

    [Test, MoqAutoData]
    public void When_ModelStateIsInvalid_Then_RouteIsPostConfirmAddProviderLocationEditCourse(
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem,
        ProviderLocationDetailsSubmitModel submitModel,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));
        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = sut.ConfirmLocation(submitModel, _learningType, larsCode).Result as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.Route.Should().Be(RouteNames.PostConfirmAddProviderLocationEditCourse);
    }

    [Test, MoqAutoData]
    public void When_ModelStateIsInvalid_Then_ShowCancelButtonIsFalse(
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem,
        ProviderLocationDetailsSubmitModel submitModel,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));
        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = sut.ConfirmLocation(submitModel, _learningType, larsCode).Result as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.ShowCancelOption.Should().BeFalse();
    }

    [Test, MoqAutoData]
    public void When_ModelStateIsInvalid_Then_IsAddJourneyIsFalse(
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem,
        ProviderLocationDetailsSubmitModel submitModel,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));
        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = sut.ConfirmLocation(submitModel, _learningType, larsCode).Result as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.IsAddJourney.Should().Be(false);
    }

    [Test, MoqAutoData]
    public void When_ModelStateIsInvalid_Then_SubmitButtonTextIsConfirm(
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem,
        ProviderLocationDetailsSubmitModel submitModel,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));
        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = sut.ConfirmLocation(submitModel, _learningType, larsCode).Result as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.SubmitButtonText.Should().Be(ButtonText.Confirm);
    }

    [Test, MoqAutoData]
    public void When_ModelStateIsInvalid_Then_CorrectDisplayHeaderIsReturned(
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem,
        ProviderLocationDetailsSubmitModel submitModel,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));
        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = sut.ConfirmLocation(submitModel, _learningType, larsCode).Result as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.DisplayHeader.Should().Be("Manage a standard");
    }

    [Test, MoqAutoData]
    public void When_ModelStateIsInvalid_Then_ReturnsExpectedCancelLinkUrl(
        Mock<ITempDataDictionary> tempDataMock,
        Mock<IUrlHelper> urlHelperMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem,
        ProviderLocationDetailsSubmitModel submitModel,
        string cancelLinkUrl,
        string larsCode)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.CancelAddProviderLocation, cancelLinkUrl);

        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = sut.ConfirmLocation(submitModel, _learningType, larsCode).Result as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.CancelLink.Should().Be(cancelLinkUrl);
    }

    [Test, MoqAutoData]
    public void When_AddressMissingInTempData_Then_RedirectsToGetProviderCourseLocations(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel submitModel,
        string larsCode)
    {
        // Arrange
        object address = null;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        //Act
        var result = sut.ConfirmLocation(submitModel, _learningType, larsCode).Result as RedirectToRouteResult;

        //Assert
        result.RouteName.Should().Be(RouteNames.GetProviderCourseLocations);
    }

    [Test, MoqAutoData]
    public async Task When_ModelStateIsValid_Then_VerifyCreateProviderLocationCommandMediatorIsInvoked(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel submitModel,
        AddressItem addressItem,
        GetAllProviderLocationsQueryResult queryResult,
        GetAllProviderLocationsQueryResult providerLocationsApiResponse,
        string larsCode)
    {
        // Arrange
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);
        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(providerLocationsApiResponse);
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        await sut.ConfirmLocation(submitModel, _learningType, larsCode);

        // Assert
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
    public async Task When_ModelStateIsValid_Then_VerifyGetAllProviderLocationsQueryMediatorIsInvoked(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel submitModel,
        AddressItem addressItem,
        GetAllProviderLocationsQueryResult queryResult,
        GetAllProviderLocationsQueryResult providerLocationsApiResponse,
        string larsCode)
    {
        // Arrange
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);
        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(providerLocationsApiResponse);
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        await sut.ConfirmLocation(submitModel, _learningType, larsCode);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task When_ModelStateIsValid_Then_VerifyTempDataIsRemoved(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel submitModel,
        AddressItem addressItem,
        GetAllProviderLocationsQueryResult queryResult,
        GetAllProviderLocationsQueryResult providerLocationsApiResponse,
        string larsCode)
    {
        // Arrange
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);
        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(providerLocationsApiResponse);
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        await sut.ConfirmLocation(submitModel, _learningType, larsCode);

        // Assert
        tempDataMock.Verify(t => t.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey));
    }

    [Test, MoqAutoData]
    public void When_ModelStateIsValid_Then_RedirectsToGetProviderCourseLocations(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel submitModel,
        AddressItem addressItem,
        GetAllProviderLocationsQueryResult queryResult,
        GetAllProviderLocationsQueryResult providerLocationsApiResponse,
        string larsCode)
    {
        // Arrange
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);
        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(providerLocationsApiResponse);
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        var result = sut.ConfirmLocation(submitModel, _learningType, larsCode).Result as RedirectToRouteResult;

        // Assert
        result.RouteName.Should().Be(RouteNames.GetProviderCourseLocations);
    }

    [Test, MoqAutoData]
    public async Task When_AddressHasEmptyFields_Then_VerifyCreateProviderLocationCommandMediatorIsInvokedWithEmptyFields(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel submitModel,
        AddressItem addressItem,
        GetAllProviderLocationsQueryResult providerLocationsApiResponse,
        string larsCode)
    {
        // Arrange
        addressItem.AddressLine2 = null;
        addressItem.Town = null;
        addressItem.County = null;
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));
        mediatorMock.Setup(m => m.Send(It.Is<GetAllProviderLocationsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(providerLocationsApiResponse);

        // Act
        await sut.ConfirmLocation(submitModel, _learningType, larsCode);

        // Assert
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
    }

    [Test, MoqAutoData]
    public async Task When_LocationNameIsNotDistinct_Then_ModelStateErrorIsAdded(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel model,
        AddressItem addressItem,
        GetAllProviderLocationsQueryResult allLocations,
        string larsCode)
    {
        // Arrange
        allLocations.ProviderLocations.FirstOrDefault().LocationName = model.LocationName;
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(allLocations);
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        await sut.ConfirmLocation(model, _learningType, larsCode);

        // Assert
        sut.ModelState.ErrorCount.Should().Be(1);
        sut.ModelState.Should().ContainKey("LocationName");
    }
}
