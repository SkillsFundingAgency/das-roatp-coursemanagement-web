using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
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

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ConfirmAddProviderLocationControllerTests.ConfirmLocationTests;

public class WhenInAddShortCourseJourney_AndPostingLocationConfirmation
{
    private ApprenticeshipType _learningType;

    [SetUp]
    public void Before_Each_Test()
    {
        _learningType = ApprenticeshipType.ApprenticeshipUnit;
    }

    [Test, MoqAutoData]
    public void When_AddressMissingInTempData_Then_RedirectsToReviewYourDetails(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel submitModel)
    {
        // Arrange
        object address = null;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        //Act
        var result = sut.ConfirmLocation(submitModel, _learningType).Result as RedirectToRouteResult;

        //Assert
        result.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }

    [Test, MoqAutoData]
    public async Task When_ModelIsInvalid_Then_ReturnsExpectedViewModel(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem,
        ProviderLocationDetailsSubmitModel submitModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = await sut.ConfirmLocation(submitModel, _learningType) as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        result.ViewName.Should().Be(ConfirmAddProviderLocationController.ViewPath);
        result.Model.Should().BeOfType<ConfirmAddProviderLocationViewModel>();
    }

    [Test, MoqAutoData]
    public async Task When_ModelIsInvalid_Then_ReturnExpectedAddress(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem,
        ProviderLocationDetailsSubmitModel submitModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = await sut.ConfirmLocation(submitModel, _learningType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.AddressLine1.Should().Be(addressItem.AddressLine1);
        model!.AddressLine2.Should().Be(addressItem.AddressLine2);
        model!.Town.Should().Be(addressItem.Town);
        model!.Postcode.Should().Be(addressItem.Postcode);
    }

    [Test, MoqAutoData]
    public async Task When_ModelIsInvalid_Then_RouteIsPostConfirmAddProviderLocation(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem,
        ProviderLocationDetailsSubmitModel submitModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = await sut.ConfirmLocation(submitModel, _learningType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.Route.Should().Be(RouteNames.PostConfirmAddProviderLocation);
    }

    [Test, MoqAutoData]
    public async Task When_ModelIsInvalid_Then_IsAddJourneyIsTrue(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem,
        ProviderLocationDetailsSubmitModel submitModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = await sut.ConfirmLocation(submitModel, _learningType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.IsAddJourney.Should().BeTrue();
    }

    [Test, MoqAutoData]
    public async Task When_ModelIsInvalid_Then_ReturnsExpectedDisplayHeader(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem,
        ProviderLocationDetailsSubmitModel submitModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = await sut.ConfirmLocation(submitModel, _learningType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model.DisplayHeader.Should().Be("Add an apprenticeship unit");
    }

    [Test, MoqAutoData]
    public async Task When_ModelIsInvalid_Then_ReturnsExpectedCancelLinkUrl(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        Mock<IUrlHelper> urlHelperMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem,
        string cancelLinkUrl,
        ProviderLocationDetailsSubmitModel submitModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.CancelAddProviderLocation, cancelLinkUrl);

        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = await sut.ConfirmLocation(submitModel, _learningType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.CancelLink.Should().Be(cancelLinkUrl);
    }

    [Test, MoqAutoData]
    public async Task When_HasSeenSummaryPageIsTrue_Then_SubmitButtonTextIsConfirm(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem,
        ProviderLocationDetailsSubmitModel submitModel)
    {
        // Arrange
        sessionModel.HasSeenSummaryPage = true;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = await sut.ConfirmLocation(submitModel, _learningType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model.SubmitButtonText.Should().Be(ButtonText.Confirm);
    }

    [Test, MoqAutoData]
    public async Task When_HasSeenSummaryPageIsFalse_Then_SubmitButtonTextIsContinue(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem,
        ProviderLocationDetailsSubmitModel submitModel)
    {
        // Arrange
        sessionModel.HasSeenSummaryPage = false;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = await sut.ConfirmLocation(submitModel, _learningType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model.SubmitButtonText.Should().Be(ButtonText.Continue);
    }

    [Test, MoqAutoData]
    public async Task When_HasSeenSummaryPageIsTrue_Then_ShowCancelButtonIsFalse(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem,
        ProviderLocationDetailsSubmitModel submitModel)
    {
        // Arrange
        sessionModel.HasSeenSummaryPage = true;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = await sut.ConfirmLocation(submitModel, _learningType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model.ShowCancelOption.Should().BeFalse();
    }

    [Test, MoqAutoData]
    public async Task When_HasSeenSummaryPageIsFalse_Then_ShowCancelButtonIsTrue(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem,
        ProviderLocationDetailsSubmitModel submitModel)
    {
        // Arrange
        sessionModel.HasSeenSummaryPage = false;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.ModelState.AddModelError("key", "message");

        // Act
        var result = await sut.ConfirmLocation(submitModel, _learningType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model.ShowCancelOption.Should().BeTrue();
    }

    [Test, MoqAutoData]
    public async Task When_ModelIsValid_Then_InvokesMediatorWithCreateCommand(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<ProviderLocationDetailsSubmitModel>> validator,
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
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        validator.Setup(x => x.Validate(It.IsAny<ProviderLocationDetailsSubmitModel>())).Returns(new ValidationResult());

        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        await sut.ConfirmLocation(submitModel, _learningType);

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
    public async Task When_ModelIsValid_Then_VerifyTempDataIsRemoved(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<ProviderLocationDetailsSubmitModel>> validator,
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
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        validator.Setup(x => x.Validate(It.IsAny<ProviderLocationDetailsSubmitModel>())).Returns(new ValidationResult());

        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        await sut.ConfirmLocation(submitModel, _learningType);

        // Assert
        tempDataMock.Verify(t => t.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey));
    }

    [Test, MoqAutoData]
    public async Task When_ModelIsValid_Then_VerifyGetAllProviderLocationsMediatorIsInvoked(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<ProviderLocationDetailsSubmitModel>> validator,
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
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        validator.Setup(x => x.Validate(It.IsAny<ProviderLocationDetailsSubmitModel>())).Returns(new ValidationResult());

        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        await sut.ConfirmLocation(submitModel, _learningType);

        // Assert
        mediatorMock.Verify(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Test, MoqAutoData]
    public async Task When_ModelIsValid_Then_VerifyGetSessionIsInvoked(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<ProviderLocationDetailsSubmitModel>> validator,
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
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        validator.Setup(x => x.Validate(It.IsAny<ProviderLocationDetailsSubmitModel>())).Returns(new ValidationResult());

        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        await sut.ConfirmLocation(submitModel, _learningType);

        // Assert
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Exactly(2));
    }

    [Test, MoqAutoData]
    public async Task When_ModelIsValid_Then_VerifySetSessionIsInvoked(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<ProviderLocationDetailsSubmitModel>> validator,
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
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        validator.Setup(x => x.Validate(It.IsAny<ProviderLocationDetailsSubmitModel>())).Returns(new ValidationResult());

        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        await sut.ConfirmLocation(submitModel, _learningType);

        // Assert
        sessionServiceMock.Verify(s => s.Set(It.Is<ShortCourseSessionModel>(m => m.TrainingVenues.FirstOrDefault().LocationName == queryResult.ProviderLocations.FirstOrDefault().LocationName && m.LocationsAvailable)), Times.Once);
    }

    [Test, MoqAutoData]
    public void When_ModelIsValid_Then_RedirectsToReviewShortCourseDetails(
    Mock<ITempDataDictionary> tempDataMock,
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Frozen] Mock<IValidator<ProviderLocationDetailsSubmitModel>> validator,
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
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        validator.Setup(x => x.Validate(It.IsAny<ProviderLocationDetailsSubmitModel>())).Returns(new ValidationResult());
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        var result = sut.ConfirmLocation(submitModel, _learningType).Result as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result.RouteName.Should().Be(RouteNames.ReviewShortCourseDetails);
    }

    [Test, MoqAutoData]
    public void When_SessionContainsEmployerLocationOption_Then_RedirectsToConfirmNationalProviderDelivery(
    Mock<ITempDataDictionary> tempDataMock,
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Frozen] Mock<IValidator<ProviderLocationDetailsSubmitModel>> validator,
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
        validator.Setup(x => x.Validate(It.IsAny<ProviderLocationDetailsSubmitModel>())).Returns(new ValidationResult());
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        var result = sut.ConfirmLocation(submitModel, apprenticeshipType).Result as RedirectToRouteResult;

        // Assert
        result.RouteName.Should().Be(RouteNames.ConfirmNationalDelivery);
    }

    [Test, MoqAutoData]
    public void When_HasNationalDeliveryOptionIsFalseAndRegionsMissing_Then_RedirectsToSelectShortCourseRegions(
    Mock<ITempDataDictionary> tempDataMock,
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Frozen] Mock<IValidator<ProviderLocationDetailsSubmitModel>> validator,
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
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);
        validator.Setup(x => x.Validate(It.IsAny<ProviderLocationDetailsSubmitModel>())).Returns(new ValidationResult());
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        var result = sut.ConfirmLocation(submitModel, _learningType).Result as RedirectToRouteResult;

        // Assert
        result.RouteName.Should().Be(RouteNames.SelectShortCourseRegions);
    }

    [Test, MoqAutoData]
    public async Task When_AddressHasEmptyFieldsAndModelStateIsValid_Then_InvokesMediatorWithCreateCommand(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<ProviderLocationDetailsSubmitModel>> validator,
        [Greedy] ConfirmAddProviderLocationController sut,
        ProviderLocationDetailsSubmitModel submitModel,
        AddressItem addressItem)
    {
        // Arrange
        validator.Setup(x => x.Validate(It.IsAny<ProviderLocationDetailsSubmitModel>())).Returns(new ValidationResult());
        addressItem.AddressLine2 = null;
        addressItem.Town = null;
        addressItem.County = null;
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        await sut.ConfirmLocation(submitModel, _learningType);

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
    public async Task When_LocationNameIsNotDistinct_Then_AddsErrorToModelState(
    Mock<ITempDataDictionary> tempDataMock,
    [Frozen] Mock<IMediator> mediatorMock,
    [Frozen] Mock<IValidator<ProviderLocationDetailsSubmitModel>> validator,
    [Greedy] ConfirmAddProviderLocationController sut,
    ProviderLocationDetailsSubmitModel model,
    AddressItem addressItem,
    GetAllProviderLocationsQueryResult allLocations)
    {
        // Arrange
        allLocations.ProviderLocations.FirstOrDefault().LocationName = model.LocationName;
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProviderLocationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(allLocations);
        validator.Setup(x => x.Validate(It.IsAny<ProviderLocationDetailsSubmitModel>())).Returns(new ValidationResult());
        object address = JsonSerializer.Serialize(addressItem);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        await sut.ConfirmLocation(model, _learningType);

        // Assert
        sut.ModelState.ErrorCount.Should().Be(1);
        sut.ModelState.Should().ContainKey("LocationName");
    }

    [Test, MoqAutoData]
    public async Task When_SessionIsNull_Then_RedirectsToReviewYourDetails(
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
        var result = await sut.ConfirmLocation(model, _learningType) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }
}
