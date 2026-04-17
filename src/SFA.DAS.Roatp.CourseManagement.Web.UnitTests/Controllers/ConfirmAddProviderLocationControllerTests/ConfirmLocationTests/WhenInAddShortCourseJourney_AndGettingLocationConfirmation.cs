using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ConfirmAddProviderLocationControllerTests.ConfirmLocationTests;
public class WhenInAddShortCourseJourney_AndGettingLocationConfirmation
{
    private ApprenticeshipType _learningType;

    [SetUp]
    public void Before_Each_Test()
    {
        _learningType = ApprenticeshipType.ApprenticeshipUnit;
    }

    [Test, MoqAutoData]
    public async Task When_SessionIsValid_Then_ReturnsExpectedViewModel(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

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
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

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
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

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
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

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
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.ConfirmLocation(_learningType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model.DisplayHeader.Should().Be("Add an apprenticeship unit");
    }

    [Test, MoqAutoData]
    public async Task When_SessionIsValid_Then_ReturnsExpectedCancelLinkUrl(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        Mock<IUrlHelper> urlHelperMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem,
        string cancelLinkUrl)
    {
        // Arrange
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.CancelAddProviderLocation, cancelLinkUrl);

        // Act
        var result = await sut.ConfirmLocation(_learningType) as ViewResult;

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
        AddressItem addressItem)
    {
        // Arrange
        sessionModel.HasSeenSummaryPage = true;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.ConfirmLocation(_learningType) as ViewResult;

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
        AddressItem addressItem)
    {
        // Arrange
        sessionModel.HasSeenSummaryPage = false;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.ConfirmLocation(_learningType) as ViewResult;

        // Assert
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model.SubmitButtonText.Should().Be(ButtonText.Continue);
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

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = await sut.ConfirmLocation(_learningType) as RedirectToRouteResult;

        // Assert
        result!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
    }

    [Test, MoqAutoData]
    public async Task When_LocationsAvailableIsTrueInSession_Then_RedirectsToSelectShortCourseTrainingVenue(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        sessionModel.LocationsAvailable = true;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var result = await sut.ConfirmLocation(_learningType) as RedirectToRouteResult;

        // Assert
        result!.RouteName.Should().Be(RouteNames.SelectShortCourseTrainingVenue);
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
